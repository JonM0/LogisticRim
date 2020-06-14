using System;
using System.Linq;
using System.Collections.Generic;
using Verse;

namespace LogisticRim
{
    public class LogisticRequester : LogisticInterface
    {
        private ThingDefCountClass request;

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
                if ( value == 0 )
                {
                    this.Remove();
                }
                else
                {
                    this.request.count = value;
                }
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
                    List<Thing> list = this.manager.map.listerThings.ThingsOfDef( this.request.thingDef );
                    for ( int i = 0; neededAmount > 0 && i < list.Count; i++ )
                    {
                        neededAmount -= list[i].stackCount;
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

        public bool CanAcceptThing ( Thing thing )
        {
            return thing.def == this.ThingDef;
        }

        public ShipmentItem CreateShipment ( LogisticManager sender )
        {
            return new ShipmentItem( this, sender, this.Missing );
        }

        override public void ExposeData ()
        {
            base.ExposeData();

            Scribe_Deep.Look( ref request, "request" );
        }
    }
}