using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LogisticRim.DebugTools
{
    internal static class CreateLogisticManager
    {
        [DebugAction( "Logistics", "Create manager", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap )]
        private static void Apply ()
        {
            Map map = Find.CurrentMap;

            if ( map.GetComponent<LogisticManager>() == null )
            {
                map.components.Add( new LogisticManager( map ) );
                Messages.Message( "Created logistic manager for map " + map.GetUniqueLoadID(), MessageTypeDefOf.NeutralEvent, false );
            }
            else
            {
                Messages.Message( "Logistic manager already present in map " + map.GetUniqueLoadID(), MessageTypeDefOf.NeutralEvent, false );
            }
        }
    }
}