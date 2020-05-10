using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace LogisticRim
{
    internal class LogisticManager : MapComponent, ILoadReferenceable
    {
        public List<LogisticInterface> interfaces = new List<LogisticInterface>();

        public LogisticManager ( Map map ) : base( map )
        {
        }

        public void AddInterface ( LogisticInterface lInterface )
        {
            interfaces.Add( lInterface );
            lInterface.manager = this;

            lInterface.loadid = this.GetUniqueLoadID() + "_" + (++rollingLoadId);
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
                shipment.channel.activeShipments.Add( shipment );
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

                    Shipment shipment = new Shipment( reqGroup.Key, this, channel );
                    shipment.AddAllShippable( this.ThingsProvided( channel ), reqGroup );
                    if ( !shipment.IsEmpty )
                    {
                        yield return shipment;
                    }
                }
            }
        }

        // active shipments

        public ICollection<Shipment> shipmentsPlanned = new HashSet<Shipment>();
        public ICollection<Shipment> shipmentsLoading = new HashSet<Shipment>();
        public ICollection<Shipment> shipmentsReady = new HashSet<Shipment>();

        // save load

        public override void ExposeData ()
        {
            base.ExposeData();

            Scribe_Collections.Look( ref interfaces, "interfaces", LookMode.Deep );

            Scribe_Values.Look( ref rollingLoadId, "rlID" );

            Scribe_Values.Look( ref ticksLastDeliveryScan, "ticksLastDeliveryScan" );
            Scribe_Values.Look( ref deliveryScanInterval, "deliveryScanInterval" );
        }

        private int rollingLoadId;

        public string GetUniqueLoadID ()
        {
            return map.GetUniqueLoadID() + "_LogisticComponent";
        }

        public override void MapGenerated ()
        {
            base.MapGenerated();

            rollingLoadId = 0;
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