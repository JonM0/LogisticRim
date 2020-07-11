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
    internal class Dialog_EditProviderPassive : Window
    {
        private LogisticProviderPassive logisticProviderPassive;

        public Dialog_EditProviderPassive ( LogisticProviderPassive logisticProviderPassive )
        {
            this.logisticProviderPassive = logisticProviderPassive;


            this.doCloseX = true;
            this.closeOnClickedOutside = true;
        }

        private Vector2 scrollPositionThingFilter;

        public override void DoWindowContents ( Rect inRect )
        {
            GUI.BeginGroup( inRect );

            ThingFilterUI.DoThingFilterConfigWindow( new Rect( 0f, 20f, inRect.width, inRect.height - 22f ), ref this.scrollPositionThingFilter, this.logisticProviderPassive.thingFilter );

            GUI.EndGroup();
        }
    }
}