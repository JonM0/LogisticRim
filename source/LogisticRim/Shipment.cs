using RimWorld;
using System.Collections.Generic;
using Verse;

namespace LogisticRim
{
    internal class Shipment : IExposable
    {
        public LogisticManager destination;
        public LogisticManager sender;
        public LogisticChannel channel;
        public List<ShipmentItem> items = new List<ShipmentItem>();

        public Shipment ( LogisticManager destination, LogisticManager sender, LogisticChannel channel )
        {
            this.destination = destination;
            this.sender = sender;
            this.channel = channel;
        }

        public bool TryGetShipmentItemForDef ( ThingDef thingDef, LogisticRequester requester, out ShipmentItem item )
        {
            item = this.items.Find( i => i.requester.ThingDef == thingDef && i.requester == requester );
            return item != null;
        }

        public void AddItem ( LogisticRequester requester, Thing thing )
        {
            if ( this.TryGetShipmentItemForDef( thing.def, requester, out ShipmentItem item ) )
            {
                item.InsertThing( thing );
            }
            else
            {
                ShipmentItem newItem = new ShipmentItem( requester, this.sender );
                newItem.InsertThing( thing );
                this.items.Add( newItem );
            }
        }

        public void AddAllShippable ( IEnumerable<Thing> source, IEnumerable<LogisticRequester> requesters )
        {
            foreach ( var thing in source )
            {
                foreach ( var requester in requesters )
                {
                    if ( requester.CanAcceptThing( thing ) )
                    {
                        this.AddItem( requester, thing );
                    }
                }
            }
        }

        public void Execute ()
        {
            List<ActiveDropPodInfo> pods = new List<ActiveDropPodInfo>();

            foreach ( var item in this.items )
            {
                item.Execute( pods );
            }

            foreach ( var pod in pods )
            {
                DropPodUtility.MakeDropPodAt( this.destination.map.Center, this.destination.map, pod );
            }
        }

        public void ExposeData ()
        {
            Scribe_References.Look( ref this.destination, "destination" );
            Scribe_References.Look( ref this.sender, "sender" );
            Scribe_References.Look( ref this.channel, "channel" );

            Scribe_Collections.Look( ref this.items, "items", LookMode.Deep );

            Scribe_Values.Look( ref this.status, "status" );
        }

        public bool IsEmpty => this.items.NullOrEmpty() || this.items.TrueForAll( i => i.Empty );

        // status

        internal enum ShipmentStatus
        {
            Planned,
            InLoading,
            ReadyToLaunch,
            InTransit,
            Complete,
        }

        private ShipmentStatus status;

        public ShipmentStatus Status
        {
            get => this.status;
            set
            {
                if ( this.status == value )
                {
                    return;
                }

                // new status
                switch ( value )
                {
                    case ShipmentStatus.Planned:
                        this.sender.shipmentsPlanned.Add( this );
                        break;

                    case ShipmentStatus.InLoading:
                        this.sender.shipmentsLoading.Add( this );
                        break;

                    case ShipmentStatus.ReadyToLaunch:
                        this.sender.shipmentsReady.Add( this );
                        break;

                    case ShipmentStatus.InTransit:
                        break;

                    case ShipmentStatus.Complete:
                        this.channel.activeShipments.Remove( this );
                        break;

                    default:
                        break;
                }

                // clean old
                switch ( this.status )
                {
                    case ShipmentStatus.Planned:
                        this.sender.shipmentsPlanned.Remove( this );
                        break;

                    case ShipmentStatus.InLoading:
                        this.sender.shipmentsLoading.Remove( this );
                        break;

                    case ShipmentStatus.ReadyToLaunch:
                        this.sender.shipmentsReady.Remove( this );
                        break;

                    case ShipmentStatus.InTransit:
                        break;

                    case ShipmentStatus.Complete:
                        break;

                    default:
                        break;
                }

                this.status = value;

            }
        }
    }
}