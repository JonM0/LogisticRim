using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimWorld;

namespace LogisticRim
{
    internal class PassiveProviderColumnWorker_ThingLabel : ColumnWorker_DataGetter<LogisticProviderPassive>
    {
        public override string GetData ( LogisticProviderPassive entry )
        {
            return entry.thingFilter.Summary;
        }
    }
}