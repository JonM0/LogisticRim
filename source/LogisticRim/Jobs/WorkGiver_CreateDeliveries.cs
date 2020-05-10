using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using Verse.AI;

namespace LogisticRim
{
    internal class WorkGiver_CreateDeliveries : WorkGiver_Scanner
    {
        public override Job JobOnThing ( Pawn pawn, Thing t, bool forced = false )
        {
            Job job = JobMaker.MakeJob( JobDefOf.CreateDeliveries, t );
            job.ignoreForbidden = true;
            return job;
        }

        public override IEnumerable<Thing> PotentialWorkThingsGlobal ( Pawn pawn )
        {
            return pawn.Map.listerBuildings
                .AllBuildingsColonistOfClass<Building_CommsConsole>()
                .Where( cc => cc.CanUseCommsNow )
                .Where( t => pawn.CanReserve( t ) );
        }

        public override bool ShouldSkip ( Pawn pawn, bool forced = false )
        {
            bool canScan = pawn.Map?.GetComponent<LogisticManager>()?.CanScan() ?? false;
            return !canScan;
        }

        [DefOf]
        private class JobDefOf
        {
            public static JobDef CreateDeliveries;

            static JobDefOf ()
            {
                DefOfHelper.EnsureInitializedInCtor( typeof( JobDefOf ) );
            }
        }
    }
}