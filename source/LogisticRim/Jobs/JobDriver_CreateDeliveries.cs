using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace LogisticRim
{
    public class JobDriver_CreateDeliveries : JobDriver
    {
        protected float BaseWorkAmount
        {
            get
            {
                return 200;
            }
        }

        public override bool TryMakePreToilReservations ( bool errorOnFailed )
        {
            return this.pawn.Reserve( this.job.targetA, this.job, 1, -1, null, errorOnFailed );
        }

        protected override IEnumerable<Toil> MakeNewToils ()
        {
            this.FailOnDespawnedNullOrForbidden( TargetIndex.A );

            yield return Toils_Goto.GotoCell( TargetIndex.A, PathEndMode.InteractionCell );

            Toil wait = Toils_General.WaitWith( TargetIndex.A, (int)(this.BaseWorkAmount / pawn.GetStatValue( StatDefOf.ResearchSpeed, true )), true, true );
            wait.FailOnCannotTouch( TargetIndex.A, PathEndMode.InteractionCell );
            wait.activeSkill = (() => SkillDefOf.Intellectual);
            wait.socialMode = RandomSocialMode.Off;
            yield return wait;

            yield return Toils_General.Do( this.DoEffect );

            yield break;
        }

        protected void DoEffect ()
        {
            this.pawn.Map.GetComponent<LogisticManager>().Scan();
        }
    }
}