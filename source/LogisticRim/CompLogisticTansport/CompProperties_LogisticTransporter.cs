using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LogisticRim
{
    public class CompProperties_LogisticTransporter : CompProperties
    {
        public CompProperties_LogisticTransporter ()
        {
            this.compClass = typeof( CompLogisticTransporter );
        }
    }
}