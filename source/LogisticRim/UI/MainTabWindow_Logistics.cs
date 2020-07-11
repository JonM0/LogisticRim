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

        protected float TableFooterHeight => 34f;

        protected float LeftPartWidth => 512f;

        protected float TableMargin => this.Margin / 2f;

        private float MaxTableWidth
        {
            get
            {
                return Mathf.Max( new float[] { this.requesterTable.Size.x, this.passiveProviderTable.Size.x, } );
            }
        }

        private float MaxTableHeight
        {
            get
            {
                return Mathf.Max( new float[] { this.requesterTable.Size.y, this.passiveProviderTable.Size.y, } );
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
                return new Vector2(
                    this.MaxTableWidth + this.LeftPartWidth + this.Margin * 3f + this.TableMargin * 2f,
                    this.MaxTableHeight + this.TableFooterHeight + this.ExtraTopSpace + this.Margin * 2f + this.TableMargin * 3f );
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
                tabContentArea.xMin = leftArea.xMax + this.Margin;
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
            TabDrawer.DrawTabs( inRect, this.tabs, 198f );

            switch ( this.curTab )
            {
                case InterfaceTabs.Requesters:
                    this.DoRequesterTab( inRect.ContractedBy( this.TableMargin ) );
                    break;

                case InterfaceTabs.PassiveProviders:
                    this.DoPassiveProviderTab( inRect.ContractedBy( this.TableMargin ) );
                    break;
            }
        }

        protected void DoRequesterTab ( Rect inRect )
        {
            this.requesterTable.TableOnGUI( inRect.position );

            // footer

            Rect footerSpace = inRect;
            footerSpace.yMin = footerSpace.yMax - this.TableFooterHeight + this.TableMargin;

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
            this.passiveProviderTable.TableOnGUI( inRect.position );

            // footer

            Rect footerSpace = inRect;
            footerSpace.yMin = footerSpace.yMax - this.TableFooterHeight + this.TableMargin;

            WidgetRow footerWidgets = new WidgetRow( footerSpace.xMin, footerSpace.yMin, UIDirection.RightThenDown, footerSpace.width );

            if ( footerWidgets.ButtonText( "Add provider" ) )
            {
                LogisticProviderPassive newLogisticProviderPassive = new LogisticProviderPassive();

                this.CurrentManager.AddInterface( newLogisticProviderPassive );

                Find.WindowStack.Add( new Dialog_EditProviderPassive( newLogisticProviderPassive ) );

                this.SetDirty();
            }
        }

        protected Vector2Int MaxTableSize => new Vector2Int(
            UI.screenWidth - (int)(this.Margin * 3f + this.LeftPartWidth + this.TableMargin * 2f),
            (int)((float)(UI.screenHeight - 35) - this.Margin * 2f - this.ExtraTopSpace - this.TableFooterHeight - this.TableMargin * 3f) );

        private TableWidget<LogisticRequester> requesterTable;

        private TableWidget<LogisticRequester> CreateRequesterTable ()
        {
            TableWidget<LogisticRequester> table = new TableWidget<LogisticRequester>(
                LogWidgets.CreateRequesterTableDef(),
                () => this.CurrentManager.Requesters,
                this.MaxTableSize.x,
                this.MaxTableSize.y );

            return table;
        }

        private TableWidget<LogisticProviderPassive> passiveProviderTable;

        private TableWidget<LogisticProviderPassive> CreatePassiveProviderTable ()
        {
            TableWidget<LogisticProviderPassive> table = new TableWidget<LogisticProviderPassive>(
                LogWidgets.CreatePassiveProviderTableDef(),
                () => this.CurrentManager.PassiveProviders,
                this.MaxTableSize.x,
                this.MaxTableSize.y );

            return table;
        }

        public override void PostOpen ()
        {
            if ( this.requesterTable == null )
            {
                this.requesterTable = this.CreateRequesterTable();
            }
            if ( this.passiveProviderTable == null )
            {
                this.passiveProviderTable = this.CreatePassiveProviderTable();
            }
            this.SetDirty();
        }

        public void SetDirty ()
        {
            this.requesterTable.SetDirty();
            this.passiveProviderTable.SetDirty();
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
            this.tabs.Add( new TabRecord( "Requesters",
                () => this.curTab = InterfaceTabs.Requesters,
                () => this.curTab == InterfaceTabs.Requesters ) );
            this.tabs.Add( new TabRecord( "Passive Providers",
                () => this.curTab = InterfaceTabs.PassiveProviders,
                () => this.curTab == InterfaceTabs.PassiveProviders ) );
        }
    }
}