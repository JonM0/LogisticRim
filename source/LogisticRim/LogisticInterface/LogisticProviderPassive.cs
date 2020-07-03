using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LogisticRim
{
    public class LogisticProviderPassive : LogisticInterface
    {
        public ThingFilter thingFilter = new ThingFilter();

        public bool Allows ( Thing thing )
        {
            return thingFilter.Allows( thing );
        }

        override public void ExposeData ()
        {
            base.ExposeData();

            Scribe_Deep.Look( ref thingFilter, "filter" );
        }
    }
}