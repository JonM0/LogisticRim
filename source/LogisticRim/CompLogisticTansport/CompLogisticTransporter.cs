using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LogisticRim
{
    public class CompLogisticTransporter : ThingComp
    {
        public CompProperties_LogisticTransporter Props
        {
            get => (CompProperties_LogisticTransporter)this.props;
        }

        public Map Map
        {
            get
            {
                return this.parent.MapHeld;
            }
        }

        public CompLaunchable Launchable
        {
            get
            {
                if ( this.cachedCompLaunchable == null )
                {
                    this.cachedCompLaunchable = this.parent.GetComp<CompLaunchable>();
                }
                return this.cachedCompLaunchable;
            }
        }

        private CompLaunchable cachedCompLaunchable;

        public CompTransporter Transporter
        {
            get
            {
                if ( this.cachedCompTransporter == null )
                {
                    this.cachedCompTransporter = this.parent.GetComp<CompTransporter>();
                }
                return this.cachedCompTransporter;
            }
        }

        private CompTransporter cachedCompTransporter;

        public Shipment shipmentInProgress;

        // ********

        public override void Initialize ( CompProperties props )
        {
            base.Initialize( props );

            // error checking
            if ( this.Transporter == null )
            {
                Log.Error( "CompLogisticTransporter requires a CompTransporter on the same thing." );
            }
            if ( this.Launchable == null )
            {
                Log.Error( "CompLogisticTransporter requires a CompLaunchable on the same thing." );
            }
        }

        public bool AllReadyForLaunch
        {
            get => this.shipmentInProgress != null
                && this.Launchable.LoadingInProgressOrReadyToLaunch
                && this.Launchable.AllInGroupConnectedToFuelingPort
                && this.Launchable.AllFuelingPortSourcesInGroupHaveAnyFuel
                && !this.Launchable.AnyInGroupHasAnythingLeftToLoad;
        }

        public override void CompTick ()
        {
            if ( this.AllReadyForLaunch )
            {
                this.Launchable.TryLaunch( this.shipmentInProgress.destination.map.Tile, new TransportPodsArrivalAction_FinalizeShipment( this.shipmentInProgress ) );

                Log.Message( "tried to lauch" );
            }
        }

        public override string CompInspectStringExtra ()
        {
            if ( this.shipmentInProgress != null )
            {
                return "Logistic shipment in progress: " + (this.AllReadyForLaunch ? "ready for launch." : "loading.");
            }
            else
            {
                return "";
            }
        }

        public override void PostExposeData ()
        {
            base.PostExposeData();

            Scribe_References.Look( ref this.shipmentInProgress, "shipmentInProgress" );
        }
    }
}