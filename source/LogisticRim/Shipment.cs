using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace LogisticRim
{
    public class Shipment : IExposable, ILoadReferenceable
    {
        public LogisticManager destination;
        public LogisticManager sender;
        public LogisticChannel channel;
        public List<ShipmentItem> items = new List<ShipmentItem>();

        public Shipment ()
        {
        }

        public Shipment ( LogisticManager destination, LogisticManager sender, LogisticChannel channel )
        {
            this.destination = destination;
            this.sender = sender;
            this.channel = channel;

            this.Status = ShipmentStatus.InCreation;

            this.id = sender.GetUniqueLoadID() + "_shipment" + sender.GetNextShipmentID();
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

        public void SetupPods ()
        {
            // exit if status is not planned

            if ( this.Status != ShipmentStatus.Planned )
            {
                Log.Error( "Can't setup pods for not planned shipment: " + this.Status.ToString() );
                return;
            }

            List<TransferableOneWay> transferables = items.Select( t => t.transferableThings ).ToList();

            // find transporters

            List<CompLogisticTransporter> logTransporters =
                this.sender
                .TransportersForMassAndDistance( CollectionsMassCalculator.MassUsageTransferables( transferables, IgnorePawnsInventoryMode.Ignore ), this.destination.map.Tile );

            Log.Message( "Transporters found: " + logTransporters.Count );

            if ( logTransporters.NullOrEmpty() )
            {
                Log.Message( "No transporters found" );
                return;
            }

            // choose which ones to use

            logTransporters = logTransporters.GetRange( 0, 1 );

            // initialize them

            foreach ( var transporter in logTransporters )
            {
                transporter.shipmentInProgress = this;
            }

            List<CompTransporter> transporters = logTransporters.Select( t => t.Transporter ).ToList();

            TransporterUtility.InitiateLoading( transporters );

            LogTransporterUtility.DistributeItems( transferables, transporters );

            this.Status = ShipmentStatus.InLoading;
        }

        public bool IsEmpty => this.items.NullOrEmpty() || this.items.TrueForAll( i => i.Empty );

        // save load

        public void ExposeData ()
        {
            Scribe_References.Look( ref this.destination, "destination" );
            Scribe_References.Look( ref this.sender, "sender" );
            Scribe_References.Look( ref this.channel, "channel" );

            Scribe_Collections.Look( ref this.items, "items", LookMode.Deep );

            Scribe_Values.Look( ref this.status, "status" );

            Scribe_Values.Look( ref this.id, "id" );
        }

        private string id;

        public string GetUniqueLoadID ()
        {
            return id;
        }

        // status

        public enum ShipmentStatus
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
                        this.sender.shipmentsInTransit.Add( this );
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
                        this.sender.shipmentsInTransit.Remove( this );
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