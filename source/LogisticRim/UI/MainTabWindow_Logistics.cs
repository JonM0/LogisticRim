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
    public class MainTabWindow_Logistics : MainTabWindow
    {
        private Vector2 scrollPosition;

        public LogisticManager CurrentManager
        {
            get => Find.CurrentMap.GetComponent<LogisticManager>();
        }

        public override void DoWindowContents ( Rect inRect )
        {
            base.DoWindowContents( inRect );

            try
            {
                Rect rect = inRect.ContractedBy( 12f );
                rect.yMin -= 4f;
                rect.yMax += 6f;
                GUI.BeginGroup( rect );

                // title

                Rect titleRect = new Rect( 0f, 0f, rect.width, 40f );

                Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.UpperLeft;
                Widgets.Label( titleRect, "ululu" );

                // table

                Rect listRect = new Rect( 0f, titleRect.height + 6f, rect.width, rect.height );

                this.DoRequesterList( listRect );
            }
            catch ( Exception ex )
            {
                Log.Error( "Exception doing Logistics window: " + ex.ToString(), false );
            }
            finally
            {
                GUI.EndGroup();
            }
        }

        public void DoRequesterList ( Rect inRect )
        {
            List<LogisticRequester> requestersForDrawing = this.CurrentManager.Requesters.ToList();

            Text.Font = GameFont.Small;

            GUI.BeginGroup( inRect );

            float height = (float)requestersForDrawing.Count * 24f;
            float num = 0f;
            Rect viewRect = new Rect( 0f, 0f, inRect.width - 16f, height );
            Widgets.BeginScrollView( inRect, ref this.scrollPosition, viewRect, true );
            float num3 = this.scrollPosition.y - 24f;
            float num4 = this.scrollPosition.y + inRect.height;
            for ( int i = 0; i < requestersForDrawing.Count; i++ )
            {
                if ( num > num3 && num < num4 )
                {
                    LogWidgets.RequesterLabelWithOptions( new Rect( 0f, num, viewRect.width, 24f ), requestersForDrawing[i] );
                }
                num += 24f;
            }
            Widgets.EndScrollView();

            GUI.EndGroup();
        }

        //private List<TabRecord> tabs = new List<TabRecord>();

        //private enum LogisticsTab : byte
        //{
        //    Requesters,
        //    Providers,
        //}
    }
}