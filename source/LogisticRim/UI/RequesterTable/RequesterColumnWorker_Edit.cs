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
    internal class RequesterColumnWorker_Edit : ColumnWorker<LogisticRequester>
    {
        public override void DoCell ( Rect rect, LogisticRequester entry, TableWidget<LogisticRequester> table )
        {
            if(Widgets.ButtonText(rect, "Edit"))
            {
                Find.WindowStack.Add( new Dialog_EditRequester( entry ) );
            }
        }
    }
}