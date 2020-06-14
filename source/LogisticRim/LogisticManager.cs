using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace LogisticRim
{
    public class LogisticManager : MapComponent, ILoadReferenceable
    {
        public List<LogisticInterface> interfaces = new List<LogisticInterface>();

        public LogisticManager ( Map map ) : base( map )
        {
        }

        public void AddInterface ( LogisticInterface lInterface )
        {
            interfaces.Add( lInterface );
            lInterface.manager = this;

            lInterface.loadid = this.GetUniqueLoadID() + "_" + (nextInterfaceID++);
        }

        public IEnumerable<LogisticChannel> Channels
        {
            get =>
                from i in interfaces
                group i by i.channel into g
                select g.Key;
        }

        public IEnumerable<LogisticProviderPassive> PassiveProviders
        {
            get =>
                interfaces.Select( i => i as LogisticProviderPassive )
                .Where( p => p != null );
        }

        public IEnumerable<LogisticRequester> Requesters
        {
            get =>
                interfaces.Select( i => i as LogisticRequester )
                .Where( p => p != null );
        }

        public IEnumerable<Thing> ThingsProvided ( LogisticChannel channel )
        {
            return from thing in map.listerThings.ThingsInGroup( ThingRequestGroup.HaulableEver )
                   from provider in PassiveProviders
                   where provider.Allows( thing )
                   select thing;
        }

        // shipment generation

        public long ticksLastDeliveryScan = 0;
        public long deliveryScanInterval = 1200;

        public bool CanScan ()
        {
            return this.ticksLastDeliveryScan + this.deliveryScanInterval < Find.TickManager.TicksGame;
        }

        public void Scan ()
        {
            this.ticksLastDeliveryScan = Find.TickManager.TicksGame;
            Log.Message( "shipments: " + this.Channels.Count() + " channels, " + this.Channels.SelectMany( c => c.Requesters ).Count() + " requesters" );

            foreach ( var shipment in this.GenerateOutgoingShipments() )
            {
                shipment.Status = Shipment.ShipmentStatus.Planned;

                Log.Message( "Shipment planned" );
            }

            Log.Message( "Scan completed" );
        }

        public IEnumerable<Shipment> GenerateOutgoingShipments ()
        {
            foreach ( var channel in this.Channels.Where( c => c.Requesters.Any( r => r.manager != this ) ) )
            {
                Log.Message( "generating shipments on channel " + channel.name );

                foreach ( var reqGroup in channel.Requesters.GroupBy( r => r.manager ).Where( g => g.Key != this ) )
                {
                    Log.Message( "generating shipments for map " + reqGroup.Key.map.GetUniqueLoadID() );

                    Shipment shipment = reqGroup.Key.GenerateRequest( this, channel );

                    if ( shipment != null )
                    {
                        yield return shipment;
                    }
                }
            }
        }

        public Shipment GenerateRequest ( LogisticManager sender, LogisticChannel channel )
        {
            Shipment shipment = new Shipment( this, sender, channel );

            foreach ( LogisticRequester requester in this.Requesters.Where( x => x.channel == channel ) )
            {
                var item = requester.CreateShipment( sender );
                if ( item.reqAmount > 0 )
                    shipment.items.Add( item );
            }

            if ( shipment.items.Any() )
            {
                shipment.AddAllShippable();
                return shipment;
            }
            else
            {
                return null;
            }
        }

        // active shipments

        public HashSet<Shipment> shipmentsPlanned = new HashSet<Shipment>();
        public HashSet<Shipment> shipmentsLoading = new HashSet<Shipment>();
        public HashSet<Shipment> shipmentsReady = new HashSet<Shipment>();
        public HashSet<Shipment> shipmentsInTransit = new HashSet<Shipment>();

        // save load

        public override void ExposeData ()
        {
            base.ExposeData();

            Scribe_Collections.Look( ref interfaces, "interfaces", LookMode.Deep );

            Scribe_Values.Look( ref nextInterfaceID, "nextInterfaceID" );

            Scribe_Values.Look( ref ticksLastDeliveryScan, "ticksLastDeliveryScan" );
            Scribe_Values.Look( ref deliveryScanInterval, "deliveryScanInterval" );

            Scribe_Collections.Look( ref shipmentsPlanned, "shipmentsPlanned", LookMode.Deep );
            Scribe_Collections.Look( ref shipmentsLoading, "shipmentsLoading", LookMode.Deep );
            Scribe_Collections.Look( ref shipmentsReady, "shipmentsReady", LookMode.Deep );
            Scribe_Collections.Look( ref shipmentsInTransit, "shipmentsInTransit", LookMode.Deep );
        }

        private int nextInterfaceID;

        public string GetUniqueLoadID ()
        {
            return map.GetUniqueLoadID() + "_LogisticComponent";
        }

        public override void MapGenerated ()
        {
            base.MapGenerated();

            nextInterfaceID = 0;
        }

        public override void MapRemoved ()
        {
            base.MapRemoved();

            foreach ( var i in interfaces )
            {
                i.Remove();
            }
        }
    }
}