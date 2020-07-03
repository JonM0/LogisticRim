//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Verse;
//using RimWorld;
//using UnityEngine;
//using System.Threading;

//namespace LogisticRim
//{
//    public class Dialog_NewRequester : Window
//    {
//        public override void DoWindowContents ( Rect inRect )
//        {
//            if ( Widgets.CloseButtonFor( inRect ) )
//            {
//                this.Close();
//            }

//            GUI.BeginGroup( inRect );

//            Rect titleRect = new Rect( 0f, 0f, inRect.width - 26f, 24f );
//            Widgets.TextArea( titleRect, "Add Requester", true );

//            // search bar

//            Rect searchRect = new Rect( 0f, titleRect.yMax + 4f, inRect.width, 24f );
//            this.SearchBar = Widgets.TextField( searchRect, this.SearchBar );

//            // confirm button
//            Rect bottomRect = new Rect( 0f, inRect.height - 28f, inRect.width, 24f );
//            if ( Widgets.ButtonText( bottomRect, "Confirm", true, true, selectedDef != null ) )
//            {
//                this.ApplyIfValid();
//                this.Close();
//            }

//            // thingdefs

//            this.DoThingDefList( new Rect( 0f, searchRect.yMax + 4f, inRect.width, bottomRect.y - searchRect.yMax - 8f ) );

//            GUI.EndGroup();
//        }

//        public Dialog_NewRequester ( MainTabWindow_Logistics mainTabWindow )
//        {
//            this.mainTabWindow = mainTabWindow;
//            this.manager = mainTabWindow.CurrentManager;

//            if ( this.manager == null )
//            {
//                Log.Error( "[LogisticRim] Null manager in Dialog_NewRequester." );
//            }
//        }

//        public void ApplyIfValid ()
//        {
//            if ( this.selectedDef != null )
//            {
//                manager.AddInterface( new LogisticRequester( this.selectedDef ) );
//                mainTabWindow.SetDirty();
//            }
//        }

//        public override void OnAcceptKeyPressed ()
//        {
//            base.OnAcceptKeyPressed();
//            if ( this.closeOnAccept )
//            {
//                this.ApplyIfValid();
//            }
//        }

//        private void DoThingDefList ( Rect inRect )
//        {
//            GUI.BeginGroup( inRect );

//            float height = (float)defSearch.Count * 24f;
//            float num = 0f;
//            Rect scrolledRect = new Rect( 0f, 0f, inRect.width - 16f, height );

//            Widgets.BeginScrollView( inRect.AtZero(), ref this.scrollPosition, scrolledRect, true );

//            float num3 = this.scrollPosition.y - 24f;
//            float num4 = this.scrollPosition.y + inRect.height;
//            for ( int i = 0; i < defSearch.Count; i++ )
//            {
//                if ( num > num3 && num < num4 )
//                {
//                    this.DoDefEntry( new Rect( 0f, num, scrolledRect.width, 24f ), defSearch[i] );
//                }
//                num += 24f;
//            }

//            Widgets.EndScrollView();

//            GUI.EndGroup();
//        }

//        private void DoDefEntry ( Rect inRect, ThingDef def )
//        {
//            Widgets.DrawOptionBackground( inRect, selectedDef == def );

//            TooltipHandler.TipRegion( inRect, def.description );

//            if ( GUI.Button( inRect, "", Widgets.EmptyStyle ) )
//            {
//                if ( this.selectedDef == def )
//                {
//                    this.selectedDef = null;
//                }
//                else
//                {
//                    this.selectedDef = def;
//                }
//            }

//            GUI.BeginGroup( inRect );

//            Rect iconRect = new Rect( 0f, 0f, inRect.height, inRect.height );
//            iconRect = iconRect.ContractedBy( 2f );
//            Widgets.DefIcon( iconRect, def, null, 1f, true );

//            Rect labelRect = new Rect( iconRect.xMax + 6f, 0f, inRect.width, inRect.height );
//            Text.Anchor = TextAnchor.MiddleLeft;
//            Text.WordWrap = false;
//            Widgets.Label( labelRect, def.LabelCap );
//            Text.Anchor = TextAnchor.UpperLeft;
//            Text.WordWrap = true;

//            GUI.EndGroup();
//        }

//        private LogisticManager manager;
//        private MainTabWindow_Logistics mainTabWindow;

//        private string searchBar = "";
//        private static readonly List<ThingDef> allDefsCached = DefDatabase<ThingDef>.AllDefsListForReading.FindAll( d => d.category == ThingCategory.Item );
//        private List<ThingDef> defSearch = allDefsCached;

//        public string SearchBar
//        {
//            get => this.searchBar;
//            set
//            {
//                if ( value != this.searchBar )
//                {
//                    if ( value == "" )
//                    {
//                        defSearch = allDefsCached;
//                    }
//                    else if ( value.Contains( this.searchBar ) )
//                    {
//                        defSearch = defSearch.FindAll( d => d.label.Contains( value ) );
//                    }
//                    else
//                    {
//                        defSearch = allDefsCached.FindAll( d => d.label.Contains( value ) );
//                    }
//                    this.searchBar = value;
//                }
//            }
//        }

//        private ThingDef selectedDef = null;
//        private Vector2 scrollPosition;
//    }
//}