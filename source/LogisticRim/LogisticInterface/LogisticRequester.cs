using System;
using System.Linq;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace LogisticRim
{
    public class LogisticRequester : LogisticInterface
    {
        private ThingDefCountClass request;

        public int activeRequest;

        public LogisticRequester ( ThingDef thingDef, int count = 0 )
        {
            this.request = new ThingDefCountClass( thingDef, count );
        }

        public LogisticRequester ()
        {
        }

        public ThingDef ThingDef => this.request.thingDef;

        public int Count
        {
            get => this.request.count;
            set
            {
                //if ( value == 0 )
                //{
                //    this.Remove();
                //}
                //else
                //{
                    this.request.count = value;
                //}
            }
        }

        public int Missing
        {
            get
            {
                int neededAmount = this.request.count;

                if ( this.request.count > 0 )
                {
                    var list = this.channel.ActiveShipments.SelectMany( s => s.items ).Where( s => s.requester == this );

                    foreach ( var e in list )
                    {
                        neededAmount -= e.Count;
                    }
                }

                if ( this.request.count > 0 )
                {
                    foreach ( var thing in this.manager.map.listerThings.AllThings.Where( this.CanAcceptThing ) )
                    {
                        neededAmount -= thing.stackCount;
                    }
                }

                if ( this.request.count > 0 )
                {
                    foreach ( var pod in this.manager.map.listerThings.ThingsInGroup( ThingRequestGroup.ActiveDropPod ).Cast<ActiveDropPod>() )
                    {
                        foreach ( var thing in pod.Contents.innerContainer.Where( this.CanAcceptThing ) )
                        {
                            neededAmount -= thing.stackCount;
                        }
                    }
                }

                return Math.Max( neededAmount, 0 );
            }
        }

        public int Planned
        {
            get
            {
                int planned = 0;

                var list = this.channel.ActiveShipments.SelectMany( s => s.items ).Where( s => s.requester == this );

                foreach ( var e in list )
                {
                    planned += e.Count;
                }

                return planned;
            }
        }

        public void UpdateRequest ()
        {
            this.activeRequest = this.Missing;
        }

        public bool CanAcceptThing ( Thing thing )
        {
            return thing.def == this.ThingDef;
        }

        public ShipmentItem CreateShipment ( LogisticManager sender )
        {
            return new ShipmentItem( this, sender, this.activeRequest );
        }

        override public void ExposeData ()
        {
            base.ExposeData();

            Scribe_Deep.Look( ref request, "request" );
            Scribe_Values.Look( ref this.activeRequest, "activeRequest", 0 );
        }

    }
}