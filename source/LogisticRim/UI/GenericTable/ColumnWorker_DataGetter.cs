using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;
using RimWorld;
using UnityEngine;

namespace LogisticRim
{
    class ColumnWorker_DataGetter<T> : ColumnWorker<T>
    {
        public virtual string GetData ( T entry )
        {
            return entry.ToString();
        }

        public override void DoCell ( Rect rect, T entry, TableWidget<T> table )
        {
            Text.Anchor = TextAnchor.MiddleLeft;
            Text.WordWrap = false;
            Widgets.Label( rect, this.GetData( entry ) );
            Text.Anchor = TextAnchor.UpperLeft;
            Text.WordWrap = true;
        }
    }
}
