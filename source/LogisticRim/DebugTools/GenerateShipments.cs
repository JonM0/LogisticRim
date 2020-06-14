using System.Linq;

using RimWorld;
using System.Collections.Generic;
using Verse;

namespace LogisticRim.DebugTools
{
    internal class GenerateShipments
    {
        private static LogisticManager Manager => Find.CurrentMap.GetComponent<LogisticManager>();

        [DebugAction( "Logistics", "Generate shipments", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap )]
        private static void Apply ()
        {
            if ( Manager == null )
            {
                Messages.Message( "Map has null manager", MessageTypeDefOf.NeutralEvent, false );
            }
            else
            {
                List<Shipment> shipments = new List<Shipment>();
                shipments.AddRange( Manager.GenerateOutgoingShipments() );

                Log.Message( shipments.Count + " shipments generated." );

                Find.WindowStack.Add( new Dialog_DebugOptionListLister( ExecuteShipments.ViewShipments( shipments ) ) );
            }
        }
    }
}