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
    internal class RequesterColumnWorker_Remove : ColumnWorker<LogisticRequester>
    {
        public override void DoCell ( Rect rect, LogisticRequester entry, TableWidget<LogisticRequester> table )
        {
            if ( Widgets.ButtonImage( rect, LogWidgets.DeleteXIcon ) )
            {
                entry.Remove();
                table.SetDirty();
            }
        }

        public override int GetMinWidth ( TableWidget<LogisticRequester> table )
        {
            return Mathf.Max( base.GetMinWidth( table ), 30 );
        }

        public override int GetMaxWidth ( TableWidget<LogisticRequester> table )
        {
            return Mathf.Min( base.GetMaxWidth( table ), this.GetMinWidth( table ) );
        }
    }
}