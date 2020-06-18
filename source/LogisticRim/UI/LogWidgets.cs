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
    [StaticConstructorOnStartup]
    internal static class LogWidgets
    {
        public static readonly Texture2D DeleteXIcon = ContentFinder<Texture2D>.Get( "UI/Buttons/Delete", true );
        public static readonly Texture2D EditIcon = ContentFinder<Texture2D>.Get( "UI/Buttons/Rename", true );

        public static void RequesterLabelWithOptions ( Rect rect, LogisticRequester logisticRequester )
        {
            Def def = logisticRequester.ThingDef;

            Widgets.DrawHighlightIfMouseover( rect );
            TooltipHandler.TipRegion( rect, def.description );

            GUI.BeginGroup( rect );

            // icon

            Rect iconRect = new Rect( 0f, 0f, rect.height, rect.height );
            iconRect = iconRect.ContractedBy( 2f );
            Widgets.DefIcon( iconRect, def, null, 1f, true );

            // buttons

            float buttonSize = rect.height - 2f;
            Rect buttonRect = new Rect( rect.width - buttonSize - 1f, 1f, buttonSize, buttonSize );

            //  delete
            if ( Widgets.ButtonImage( buttonRect, DeleteXIcon, Color.white, GenUI.SubtleMouseoverColor ) )
            {
                logisticRequester.Remove();
            }

            //  edit
            buttonRect.x -= buttonSize + 2f;
            if ( Widgets.ButtonImage( buttonRect, EditIcon ) )
            {
                Find.WindowStack.Add(
                    new Dialog_Slider(
                        n => logisticRequester.ThingDef.defName + ": " + n,
                        0, 1500,
                        n => logisticRequester.Count = n,
                        logisticRequester.Count ) );
            }

            // slider
            float sliderWidth = 248f;
            Rect sliderRect = new Rect( buttonRect.x - sliderWidth - 2f, 0, sliderWidth, rect.height );

            logisticRequester.Count = (int)Widgets.HorizontalSlider( sliderRect, logisticRequester.Count, 0, 2000 );

            // label

            Rect rect3 = new Rect( iconRect.xMax + 6f, 0f, rect.width, rect.height );
            Text.Anchor = TextAnchor.MiddleLeft;
            Text.WordWrap = false;
            Widgets.Label( rect3, def.LabelCap );
            Text.Anchor = TextAnchor.UpperLeft;
            Text.WordWrap = true;

            GUI.EndGroup();
        }
    }
}