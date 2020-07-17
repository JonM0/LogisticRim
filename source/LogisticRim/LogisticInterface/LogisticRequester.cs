using System;
using System.Linq;
using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

namespace LogisticRim
{
    public class LogisticRequester : LogisticInterface
    {
        public ThingFilter requestFilter = new ThingFilter();

        public int totalRequestCount;
        public int activeRequestCount;
        public int urgencyThreshold;

        public LogisticRequester ( ThingFilter filter, int count = 0 )
        {
            this.requestFilter = filter;
            this.totalRequestCount = count;
            this.activeRequestCount = 0;
        }

        public LogisticRequester ()
        {
        }

        //public ThingDef ThingDef => this.request.thingDef;

        public int Count
        {
            get => this.totalRequestCount;
            set
            {
                this.totalRequestCount = Math.Max( value, 0 );
            }
        }

        /// <summary>
        /// The amount of things needed to reach the needed count
        /// </summary>
        public int Missing
        {
            get
            {
                int neededAmount = this.totalRequestCount;

                if ( neededAmount > 0 )
                {
                    // Count items that are in an active shipment
                    neededAmount -= this.Planned;
                }

                if ( neededAmount > 0 )
                {
                    // Count items in the map
                    foreach ( var thing in this.manager.map.listerThings.AllThings.Where( this.CanAcceptThing ) )
                    {
                        neededAmount -= thing.stackCount;
                    }
                }

                if ( neededAmount > 0 )
                {
                    // Count items in falling drop pods
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

        /// <summary>
        /// Amount of requested items in active shipments
        /// </summary>
        public int Planned
        {
            get
            {
                return this.channel.ActiveShipments.SelectMany( s => s.items ) // all shipment items
                    .Where( s => s.requester == this )  // created out of this requester
                    .Select( s => s.Count ).Aggregate( 0, ( a, b ) => a + b ); // sum the count
            }
        }

        public void UpdateRequest ()
        {
            this.activeRequestCount = this.Missing;
        }

        public bool CanAcceptThing ( Thing thing )
        {
            return this.requestFilter.Allows( thing );
        }

        public ShipmentItem CreateShipmentItem ( LogisticManager sender )
        {
            return new ShipmentItem( this, sender );
        }

        override public void ExposeData ()
        {
            base.ExposeData();

            Scribe_Deep.Look( ref this.requestFilter, "requestFilter" );
            Scribe_Values.Look( ref this.totalRequestCount, "totalRequestCount", 0 );
            Scribe_Values.Look( ref this.activeRequestCount, "activeRequest", 0 );
            Scribe_Values.Look( ref this.urgencyThreshold, "urgencyThreshold", 0 );
        }
    }
}