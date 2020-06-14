using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LogisticRim
{
    internal class ShipmentItem : IExposable
    {
        public LogisticRequester requester;
        public LogisticManager sender;

        public TransferableOneWay transferableThings = new TransferableOneWay();
        public int reqAmount;

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
        }

        public ShipmentItem ( LogisticRequester requester, LogisticManager sender, int amount )
        {
            this.requester = requester;
            this.sender = sender;
            this.reqAmount = amount;
        }

        public bool Empty => this.Count == 0;
    }
}