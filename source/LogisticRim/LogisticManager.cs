﻿using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace LogisticRim
{
    public class LogisticManager : MapComponent, ILoadReferenceable
    {
        private List<LogisticInterface> interfaces = new List<LogisticInterface>();

        public LogisticManager ( Map map ) : base( map )
        {
        }

        public void AddInterface ( LogisticInterface lInterface )
        {
            if ( this.interfaces.Contains( lInterface ) )
            {
                Log.Error( "[LogisticRim] Tried to add duplicate interface to manager on map: " + this.map.GetUniqueLoadID() );
                return;
            }
            if ( lInterface.manager != null )
            {
                Log.Error( "[LogisticRim] Tried to interface belonging to another manager to manager on map: " + this.map.GetUniqueLoadID() );
                return;
            }

            this.interfaces.Add( lInterface );
            lInterface.manager = this;

            lInterface.loadid = this.GetUniqueLoadID() + "_" + (this.nextInterfaceID++);
        }

        public void RemoveInterface ( LogisticInterface lInterface )
        {
            this.interfaces.Remove( lInterface );
            lInterface.manager = null;
        }

        public IEnumerable<LogisticChannel> Channels
        {
            get =>
                from i in this.interfaces
                group i by i.channel into g
                select g.Key;
        }

        public IEnumerable<LogisticProviderPassive> PassiveProviders
        {
            get =>
                this.interfaces.Select( i => i as LogisticProviderPassive )
                .Where( p => p != null );
        }

        public IEnumerable<LogisticRequester> Requesters
        {
            get =>
                this.interfaces.Select( i => i as LogisticRequester )
                .Where( p => p != null );
        }

        public IEnumerable<Thing> ThingsProvided ( LogisticChannel channel )
        {
            return from thing in this.map.listerThings.ThingsInGroup( ThingRequestGroup.HaulableEver )
                   from provider in this.PassiveProviders
                   where provider.Allows( thing )
                   select thing;
        }

        // transporters

        public List<CompLogisticTransporter> transporters = new List<CompLogisticTransporter>();

        public IEnumerable<CompLogisticTransporter> AvailableTransporters
        {
            get => transporters.Where( t => t.Available );
        }

        public List<CompLogisticTransporter> TransportersForMassAndDistance ( float mass, int tile )
        {
            float totMass = 0;
            List<CompLogisticTransporter> res = new List<CompLogisticTransporter>();
            int distance = Find.WorldGrid.TraversalDistanceBetween( this.map.Tile, tile, true, int.MaxValue );

            foreach ( var transporter in AvailableTransporters )
            {
                if ( transporter.MaxLaunchDistance >= distance )
                {
                    totMass += transporter.Transporter.Props.massCapacity;
                    res.Add( transporter );

                    if ( totMass >= mass )
                        return res;
                }
            }

            return null;
        }

        // shipment generation

        public float OutgoingShipmentTargetMass
        {
            get => this.AvailableTransporters.FirstOrDefault()?.Transporter.Props.massCapacity ?? 0f;
        }

        public float minimumPodFill = 0.8f;

        public long ticksLastDeliveryScan = 0;
        public long deliveryScanInterval = 1200;

        public bool CanScan ()
        {
            return this.ticksLastDeliveryScan + this.deliveryScanInterval < Find.TickManager.TicksGame;
        }

        public void Scan ()
        {
            this.ticksLastDeliveryScan = Find.TickManager.TicksGame;

            foreach ( var requester in this.Requesters )
            {
                requester.UpdateRequest();
            }

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

                    Shipment shipment = reqGroup.Key.GenerateIncomingShipment( this, channel );

                    if ( shipment != null )
                    {
                        yield return shipment;
                    }
                }
            }
        }

        public Shipment GenerateIncomingShipment ( LogisticManager sender, LogisticChannel channel )
        {
            Shipment shipment = new Shipment( this, sender, channel );

            foreach ( LogisticRequester requester in this.Requesters.Where( x => x.channel == channel ) )
            {
                var item = requester.CreateShipmentItem( sender );
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

        public override void MapComponentTick ()
        {
            if ( Find.TickManager.TicksGame % 719 == 0 )
            {
                base.MapComponentTick();

                foreach ( var shipment in this.shipmentsPlanned )
                {
                    shipment.SetupPods();
                    break;
                }
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

            Scribe_Collections.Look( ref this.interfaces, "interfaces", LookMode.Deep );

            Scribe_Values.Look( ref this.nextInterfaceID, "nextInterfaceID" );
            Scribe_Values.Look( ref this.nextShipmentID, "nextShipmentID" );

            Scribe_Values.Look( ref this.ticksLastDeliveryScan, "ticksLastDeliveryScan" );
            Scribe_Values.Look( ref this.deliveryScanInterval, "deliveryScanInterval" );
            Scribe_Values.Look( ref this.minimumPodFill, "minimumPodFill" );

            Scribe_Collections.Look( ref this.shipmentsPlanned, "shipmentsPlanned", LookMode.Deep );
            Scribe_Collections.Look( ref this.shipmentsLoading, "shipmentsLoading", LookMode.Deep );
            Scribe_Collections.Look( ref this.shipmentsReady, "shipmentsReady", LookMode.Deep );
            Scribe_Collections.Look( ref this.shipmentsInTransit, "shipmentsInTransit", LookMode.Deep );

            //Scribe_Collections.Look( ref this.transporters, "transporters", LookMode.Reference );
        }

        private int nextInterfaceID;
        private int nextShipmentID;

        public string GetUniqueLoadID ()
        {
            return this.map.GetUniqueLoadID() + "_LogisticComponent";
        }

        public int GetNextShipmentID ()
        {
            return this.nextShipmentID;
        }

        public override void MapGenerated ()
        {
            base.MapGenerated();

            this.nextInterfaceID = 0;
            this.nextShipmentID = 0;
        }

        public override void MapRemoved ()
        {
            base.MapRemoved();

            foreach ( var i in this.interfaces )
            {
                i.Remove();
            }
        }
    }
}