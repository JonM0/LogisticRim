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
    internal class ProviderPassiveColumnWorker_Edit : ColumnWorker<LogisticProviderPassive>
    {
        public override void DoCell ( Rect rect, LogisticProviderPassive entry, TableWidget<LogisticProviderPassive> table )
        {
            if ( Widgets.ButtonText( rect.ContractedBy( 1f ), "Edit" ) )
            {
                Find.WindowStack.Add( new Dialog_EditProviderPassive( entry ) );
            }
        }
    }
}