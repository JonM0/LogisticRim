using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LogisticRim
{
    public class ShipmentItem : IExposable
    {
        public ShipmentItem ()
        {
        }

        public LogisticRequester requester;
        public LogisticManager sender;

        public TransferableOneWay transferableThings = new TransferableOneWay();
        public int reqAmount;
        public int urgencyThreshold;

        public int Count => this.transferableThings.CountToTransfer;

        public bool TryInsertThing ( Thing thing )
        {
            if ( requester.CanAcceptThing( thing ) )
            {
                transferableThings.things.Add( thing );

                transferableThings.AdjustTo( this.reqAmount );

                return true;
            }
            else
            {
                return false;
            }
        }

        public void ExposeData ()
        {
            Scribe_References.Look( ref requester, "requester" );
            Scribe_References.Look( ref sender, "sender" );

            Scribe_Deep.Look( ref transferableThings, "transferableThings" );
            Scribe_Values.Look( ref reqAmount, "reqAmount" );
            Scribe_Values.Look( ref urgencyThreshold, "urgencyThreshold" );
        }

        public ShipmentItem ( LogisticRequester requester, LogisticManager sender )
        {
            this.requester = requester;
            this.sender = sender;

            this.reqAmount = requester.activeRequestCount;
            this.urgencyThreshold = requester.urgencyThreshold;
        }

        public bool Empty => this.Count == 0;
    }
}