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
        public LogisticManager CurrentManager
        {
            get => Find.CurrentMap.GetComponent<LogisticManager>();
        }

        protected float ExtraTopSpace => 34f;

        protected float ExtraBottomSpace => 28f;

        protected float LeftPartWidth => 128f;

        private float MaxTableWidth
        {
            get
            {
                return Mathf.Max( new float[] { this.requesterTable.Size.x, } );
            }
        }

        private float MaxTableHeight
        {
            get
            {
                return Mathf.Max( new float[] { this.requesterTable.Size.y, } );
            }
        }

        public override Vector2 RequestedTabSize
        {
            get
            {
                if ( this.requesterTable == null )
                {
                    return Vector2.zero;
                }
                return new Vector2( this.MaxTableWidth + this.LeftPartWidth + this.Margin * 3f, this.MaxTableHeight + this.ExtraBottomSpace + this.ExtraTopSpace + this.Margin * 2f );
            }
        }

        public override void DoWindowContents ( Rect inRect )
        {
            base.DoWindowContents( inRect );

            try
            {
                Rect leftArea = inRect;
                leftArea.width = this.LeftPartWidth;

                this.DoLeftPart( leftArea );

                Rect tabContentArea = inRect;
                tabContentArea.xMin += this.LeftPartWidth + this.Margin;
                tabContentArea.yMin += this.ExtraTopSpace;

                this.DoTabContent( tabContentArea );
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

        protected void DoLeftPart ( Rect inRect )
        {
        }

        protected void DoTabContent ( Rect inRect )
        {
            Widgets.DrawMenuSection( inRect );
            TabDrawer.DrawTabs( inRect, this.tabs, 200f );

            switch ( this.curTab )
            {
                case InterfaceTabs.Requesters:
                    this.DoRequesterTab( inRect );
                    break;

                case InterfaceTabs.PassiveProviders:
                    this.DoPassiveProviderTab( inRect );
                    break;
            }
        }

        protected void DoRequesterTab ( Rect inRect )
        {
            this.requesterTable.TableOnGUI( inRect.position );

            // footer

            Rect footerSpace = inRect;
            footerSpace.yMin = footerSpace.yMax - this.ExtraBottomSpace + 2;

            WidgetRow footerWidgets = new WidgetRow( footerSpace.xMin, footerSpace.yMin, UIDirection.RightThenDown, footerSpace.width );

            if ( footerWidgets.ButtonText( "Add request" ) )
            {
                LogisticRequester newLogisticRequester = new LogisticRequester( new ThingFilter() );

                this.CurrentManager.AddInterface( newLogisticRequester );

                Find.WindowStack.Add( new Dialog_EditRequester( newLogisticRequester ) );

                this.SetDirty();
            }
        }

        protected void DoPassiveProviderTab ( Rect inRect )
        {
        }

        private TableWidget<LogisticRequester> requesterTable;

        private TableWidget<LogisticRequester> CreateRequesterTable ()
        {
            TableWidget<LogisticRequester> table = new TableWidget<LogisticRequester>(
                LogWidgets.CreateRequesterTableDef(),
                new Func<IEnumerable<LogisticRequester>>( () => this.CurrentManager.Requesters ),
                UI.screenWidth - (int)(this.Margin * 3f + this.LeftPartWidth),
                (int)((float)(UI.screenHeight - 35) - this.Margin * 2f - this.ExtraTopSpace - this.ExtraBottomSpace) );

            return table;
        }

        public override void PostOpen ()
        {
            if ( this.requesterTable == null )
            {
                this.requesterTable = this.CreateRequesterTable();
            }
            this.SetDirty();
        }

        public void SetDirty ()
        {
            this.requesterTable.SetDirty();
            this.SetInitialSizeAndPosition();
        }

        private List<TabRecord> tabs = new List<TabRecord>();
        private InterfaceTabs curTab;

        private enum InterfaceTabs : byte
        {
            Requesters,
            PassiveProviders,
        }

        public override void PreOpen ()
        {
            base.PreOpen();

            this.tabs.Clear();
            this.tabs.Add( new TabRecord( "Requesters", delegate ()
            {
                this.curTab = InterfaceTabs.Requesters;
            }, () => this.curTab == InterfaceTabs.Requesters ) );
            this.tabs.Add( new TabRecord( "Passive Providers", delegate ()
            {
                this.curTab = InterfaceTabs.PassiveProviders;
            }, () => this.curTab == InterfaceTabs.PassiveProviders ) );
        }
    }
}