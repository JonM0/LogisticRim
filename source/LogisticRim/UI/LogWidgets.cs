using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace LogisticRim
{
    [StaticConstructorOnStartup]
    internal static class LogWidgets
    {
        public static readonly Texture2D DeleteXIcon = ContentFinder<Texture2D>.Get( "UI/Buttons/Delete", true );
        public static readonly Texture2D EditIcon = ContentFinder<Texture2D>.Get( "UI/Buttons/Rename", true );

        public static TableDef<LogisticRequester> CreateRequesterTableDef ()
        {
            ColumnDef<LogisticRequester> reqThing = new ColumnDef<LogisticRequester>();
            reqThing.label = "Requested thing";
            reqThing.workerClass = typeof( RequesterColumnWorker_ThingLabel );

            ColumnDef<LogisticRequester> reqAmnt = new ColumnDef<LogisticRequester>();
            reqAmnt.label = "Amount";
            reqAmnt.workerClass = typeof( RequesterColumnWorker_ReqestAmount );
            reqAmnt.sortable = true;

            ColumnDef<LogisticRequester> activeReq = new ColumnDef<LogisticRequester>();
            activeReq.label = "Active";
            activeReq.workerClass = typeof( RequesterColumnWorker_ActiveRequests );
            activeReq.headerTip = "The amount that has been actually requested";
            activeReq.sortable = true;

            ColumnDef<LogisticRequester> edit = new ColumnDef<LogisticRequester>();
            edit.headerIcon = "UI/Buttons/Rename";
            edit.workerClass = typeof( RequesterColumnWorker_Edit );

            ColumnDef<LogisticRequester> remove = new ColumnDef<LogisticRequester>();
            remove.workerClass = typeof( RequesterColumnWorker_Remove );

            TableDef<LogisticRequester> tableDef = new TableDef<LogisticRequester>();
            tableDef.columns = new List<ColumnDef<LogisticRequester>>()
            {
                reqThing, reqAmnt, activeReq, edit, remove,
            };

            return tableDef;
        }
    }
}