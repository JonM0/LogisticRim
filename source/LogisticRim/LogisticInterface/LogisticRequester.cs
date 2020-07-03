using System;
using System.Linq;
using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

namespace LogisticRim
{
    public class LogisticRequester : LogisticInterface
    {
        public ThingFilter requestFilter = new ThingFilter();

        public int totalRequestCount;
        public int activeRequestCount;

        public LogisticRequester ( ThingFilter filter, int count = 0 )
        {
            this.requestFilter = filter;
            this.totalRequestCount = count;
            this.activeRequestCount = 0;
        }

        public LogisticRequester ()
        {
        }

        //public ThingDef ThingDef => this.request.thingDef;

        public int Count
        {
            get => this.totalRequestCount;
            set
            {
                this.totalRequestCount = Math.Max( value, 0 );
            }
        }

        /// <summary>
        /// The amount of things needed to reach the needed count
        /// </summary>
        public int Missing
        {
            get
            {
                int neededAmount = this.totalRequestCount;

                if ( neededAmount > 0 )
                {
                    // Count items that are in an active shipment
                    neededAmount -= this.Planned;
                }

                if ( neededAmount > 0 )
                {
                    // Count items in the map
                    foreach ( var thing in this.manager.map.listerThings.AllThings.Where( this.CanAcceptThing ) )
                    {
                        neededAmount -= thing.stackCount;
                    }
                }

                if ( neededAmount > 0 )
                {
                    // Count items in falling drop pods
                    foreach ( var pod in this.manager.map.listerThings.ThingsInGroup( ThingRequestGroup.ActiveDropPod ).Cast<ActiveDropPod>() )
                    {
                        foreach ( var thing in pod.Contents.innerContainer.Where( this.CanAcceptThing ) )
                        {
                            neededAmount -= thing.stackCount;
                        }
                    }
                }

                return Math.Max( neededAmount, 0 );
            }
        }

        /// <summary>
        /// Amount of requested items in active shipments
        /// </summary>
        public int Planned
        {
            get
            {
                return this.channel.ActiveShipments.SelectMany( s => s.items ) // all shipment items
                    .Where( s => s.requester == this )  // created out of this requester
                    .Select( s => s.Count ).Aggregate( 0, ( a, b ) => a + b ); // sum the count
            }
        }

        public void UpdateRequest ()
        {
            this.activeRequestCount = this.Missing;
        }

        public bool CanAcceptThing ( Thing thing )
        {
            return this.requestFilter.Allows( thing );
        }

        public ShipmentItem CreateShipment ( LogisticManager sender )
        {
            return new ShipmentItem( this, sender, this.activeRequestCount );
        }

        override public void ExposeData ()
        {
            base.ExposeData();

            Scribe_Deep.Look( ref this.requestFilter, "requestFilter" );
            Scribe_Values.Look( ref this.totalRequestCount, "totalRequestCount", 0 );
            Scribe_Values.Look( ref this.activeRequestCount, "activeRequest", 0 );
        }

        //private string UIbuffer;

        //public void DoListEntry ( Rect rect )
        //{
        //    Def def = this.ThingDef;

        //    Widgets.DrawLineHorizontal( rect.x, rect.y, rect.width );
        //    Widgets.DrawHighlightIfMouseover( rect );
        //    TooltipHandler.TipRegion( rect, def.description );

        //    GUI.BeginGroup( rect );

        //    // icon

        //    Rect iconRect = new Rect( 0f, 0f, rect.height, rect.height );
        //    iconRect = iconRect.ContractedBy( 2f );
        //    Widgets.DefIcon( iconRect, def, null, 1f, true );

        //    // buttons

        //    float buttonSize = rect.height - 2f;
        //    Rect buttonRect = new Rect( rect.width - buttonSize - 1f, 1f, buttonSize, buttonSize );

        //    //  delete
        //    if ( Widgets.ButtonImage( buttonRect, LogWidgets.DeleteXIcon, Color.white, GenUI.SubtleMouseoverColor ) )
        //    {
        //        this.Remove();
        //    }

        //    // edit

        //    Widgets.TextFieldNumeric( new Rect( rect.width / 2f, 0f, 256f, rect.height ), ref this.request.count, ref this.UIbuffer );

        //    //buttonRect.x -= buttonSize + 2f;
        //    //if ( Widgets.ButtonImage( buttonRect, EditIcon ) )
        //    //{
        //    //    Find.WindowStack.Add(
        //    //        new Dialog_Slider(
        //    //            n => logisticRequester.ThingDef.defName + ": " + n,
        //    //            0, 1500,
        //    //            n => logisticRequester.Count = n,
        //    //            logisticRequester.Count ) );
        //    //}

        //    // slider
        //    //float sliderWidth = 248f;
        //    //Rect sliderRect = new Rect( buttonRect.x - sliderWidth - 2f, 0, sliderWidth, rect.height );

        //    //logisticRequester.Count = (int)Widgets.HorizontalSlider( sliderRect, logisticRequester.Count, 0, 2000 );

        //    // label

        //    Rect rect3 = new Rect( iconRect.xMax + 6f, 0f, rect.width, rect.height );
        //    Text.Anchor = TextAnchor.MiddleLeft;
        //    Text.WordWrap = false;
        //    Widgets.Label( rect3, def.LabelCap );
        //    Text.Anchor = TextAnchor.UpperLeft;
        //    Text.WordWrap = true;

        //    GUI.EndGroup();
        //}
    }
}