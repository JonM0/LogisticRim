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
    internal class RequesterColumnWorker_ThingLabel : ColumnWorker_DataGetter<LogisticRequester>
    {
        public override string GetData ( LogisticRequester entry )
        {
            return entry.requestFilter.Summary;
        }
    }
}