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
    internal class Dialog_EditRequester : Window
    {
        private LogisticRequester logisticRequester;

        public Dialog_EditRequester ( LogisticRequester logisticRequester )
        {
            this.logisticRequester = logisticRequester;

            this.editBufferCount = logisticRequester.totalRequestCount.ToString();

            this.doCloseX = true;
            this.closeOnClickedOutside = true;
        }

        private string editBufferCount;
        private Vector2 scrollPositionThingFilter;

        public override void DoWindowContents ( Rect inRect )
        {
            GUI.BeginGroup( inRect );

            ThingFilterUI.DoThingFilterConfigWindow( new Rect( 0f, 20f, inRect.width, inRect.height - 70f ), ref this.scrollPositionThingFilter, this.logisticRequester.requestFilter );

            Widgets.IntEntry( new Rect( 0f, inRect.height - 40f, inRect.width, 32f ), ref this.logisticRequester.totalRequestCount, ref this.editBufferCount );
            if ( this.logisticRequester.totalRequestCount < 0 ) this.logisticRequester.totalRequestCount = 0;

            GUI.EndGroup();
        }
    }
}