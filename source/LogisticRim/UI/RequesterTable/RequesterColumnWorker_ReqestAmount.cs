using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace LogisticRim
{
    internal class RequesterColumnWorker_ReqestAmount : ColumnWorker<LogisticRequester>
    {
        public override void DoCell ( Rect rect, LogisticRequester entry, TableWidget<LogisticRequester> table )
        {
            string buffer = entry.Count.ToString();

            int tempCount = entry.Count;
            Widgets.IntEntry( rect, ref tempCount, ref buffer );
            entry.Count = tempCount;
        }

        public override int Compare ( LogisticRequester a, LogisticRequester b )
        {
            return Comparer<int>.Default.Compare(a.Count, b.Count);
        }
    }
}