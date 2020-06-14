using RimWorld;
using System.Collections.Generic;
using System.Linq;
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

            this.Status = ShipmentStatus.InCreation;
        }

        public void AddAllShippable ()
        {
            foreach ( var thing in sender.ThingsProvided( this.channel ) )
            {
                foreach ( var shipmentItem in items )
                {
                    shipmentItem.TryInsertThing( thing );
                }
            }
        }

        public void SetupPods ( List<CompTransporter> transporters )
        {
            // exit if status is not planned

            if ( this.Status != ShipmentStatus.Planned )
            {
                Log.Error( "Can't setup pods for not planned shipment" );
                return;
            }

            if ( transporters == null )
            {
                Log.Error( "Transporters cannot be null" );
                return;
            }

            TransporterUtility.InitiateLoading( transporters );

            Dictionary<TransferableOneWay, int> tmpLeftCountToTransfer = new Dictionary<TransferableOneWay, int>();

            foreach ( var item in items )
            {
                tmpLeftCountToTransfer.Add( item.transferableThings, item.Count );
            }

            TransferableOneWay biggestTransferable = this.items.Select( i => i.transferableThings ).MaxBy( ( TransferableOneWay x ) => tmpLeftCountToTransfer[x] );

            int transporterIndex = 0;
            // load all but the biggest
            foreach ( var transferable in this.items.Select( i => i.transferableThings ) )
            {
                if ( transferable != biggestTransferable && tmpLeftCountToTransfer[transferable] > 0 )
                {
                    transporters[transporterIndex % transporters.Count].AddToTheToLoadList( transferable, tmpLeftCountToTransfer[transferable] );
                    transporterIndex++;
                }
            }
            // if there are empty pods distribute the biggest among the remaining
            if ( transporterIndex < transporters.Count )
            {
                int amountToDistribute = tmpLeftCountToTransfer[biggestTransferable];
                int amountEach = amountToDistribute / (transporters.Count - transporterIndex);
                for ( int m = transporterIndex; m < transporters.Count; m++ )
                {
                    int amountAdded = (m == transporters.Count - 1) ? amountToDistribute : amountEach; // on the last one add all the remaining
                    if ( amountAdded > 0 )
                    {
                        transporters[m].AddToTheToLoadList( biggestTransferable, amountAdded );
                    }
                    amountToDistribute -= amountAdded;
                }
            }
            // else just add it to one
            else
            {
                transporters[transporterIndex % transporters.Count].AddToTheToLoadList( biggestTransferable, tmpLeftCountToTransfer[biggestTransferable] );
            }

            this.Status = ShipmentStatus.InLoading;
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
            InCreation,
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