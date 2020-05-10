using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LogisticRim
{
    internal class LogisticProviderPassive : LogisticInterface
    {
        public ThingFilter thingFilter = new ThingFilter();

        public FilterMode mode = FilterMode.Whitelist;

        internal enum FilterMode
        {
            Whitelist,
            Blacklist
        }

        public bool Allows ( Thing thing )
        {
            switch ( mode )
            {
                case FilterMode.Whitelist:
                    return thingFilter.Allows( thing );

                case FilterMode.Blacklist:
                    return !thingFilter.Allows( thing );

                default:
                    return false;
            };
        }

        override public void ExposeData ()
        {
            base.ExposeData();

            Scribe_Deep.Look( ref thingFilter, "filter" );
            Scribe_Values.Look( ref mode, "mode" );
        }
    }
}