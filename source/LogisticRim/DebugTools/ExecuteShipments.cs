using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LogisticRim.DebugTools
{
    internal class ExecuteShipments
    {
        private static LogisticManager Manager => Find.CurrentMap.GetComponent<LogisticManager>();

        [DebugAction( "Logistics", "View active shipments", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap )]
        private static void Apply ()
        {
            if ( Manager == null )
            {
                Messages.Message( "Map has null manager", MessageTypeDefOf.NeutralEvent, false );
            }
            else
            {
                List<Shipment> shipments = new List<Shipment>();
                shipments.AddRange( Manager.Channels.SelectMany( c => c.activeShipments ) );

                Find.WindowStack.Add( new Dialog_DebugOptionListLister( ViewShipments( shipments ) ) );
            }
        }

        private static IEnumerable<DebugMenuOption> ViewShipments ( List<Shipment> shipments )
        {
            foreach ( var shipment in shipments )
            {
                yield return new DebugMenuOption
                {
                    label = "View destination: " + shipment.destination.map.GetUniqueLoadID(),
                    method = () =>
                    {
                        IEnumerable<ShipmentItem> dataSources =
                            shipment.items;

                        TableDataGetter<ShipmentItem>[] array = new TableDataGetter<ShipmentItem>[3];
                        array[0] = new TableDataGetter<ShipmentItem>( "thingDef", ( ShipmentItem r ) => r.requester.ThingDef );
                        array[1] = new TableDataGetter<ShipmentItem>( "amount", ( ShipmentItem r ) => r.Count );// thingCounts.Select( tc => tc.Count ).Aggregate( ( a, b ) => a + b ) ) ;
                        array[2] = new TableDataGetter<ShipmentItem>( "requested", ( ShipmentItem r ) => r.requester.Missing );

                        DebugTables.MakeTablesDialog( dataSources, array );
                    },
                    mode = DebugMenuOptionMode.Action,
                };

                yield return new DebugMenuOption
                {
                    label = "Send to destination: " + shipment.destination.map.GetUniqueLoadID(),
                    method = () =>
                    {
                        shipment.Execute();
                    },
                    mode = DebugMenuOptionMode.Action,
                };
            }
        }
    }
}