using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LogisticRim.DebugTools
{
    internal static class ManageInterfaces
    {
        private static LogisticManager Manager => Find.CurrentMap.GetComponent<LogisticManager>();

        [DebugAction( "Logistics", "Manage interfaces", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap )]
        private static void Apply ()
        {
            if ( Manager == null )
            {
                Messages.Message( "Map has null manager", MessageTypeDefOf.NeutralEvent, false );
            }
            else
            {
                Find.WindowStack.Add( new Dialog_DebugOptionListLister( Options_ManageInterfaces() ) );
            }
        }

        private static IEnumerable<DebugMenuOption> Options_ManageInterfaces ()
        {
            yield return Option_AddPP();
            //yield return Option_AddReq();
            foreach ( var pp in Manager.PassiveProviders )
            {
                yield return Option_EditPP( pp );
            }
            //foreach ( var r in Manager.Requesters )
            //{
            //    yield return Option_EditReq( r );
            //}
        }

        private static DebugMenuOption Option_Remove ( LogisticInterface i )
        {
            return new DebugMenuOption
            {
                label = "Remove",
                method = () =>
                    i.Remove(),
                mode = DebugMenuOptionMode.Action,
            };
        }

        // provider

        private static DebugMenuOption Option_AddPP ()
        {
            return new DebugMenuOption
            {
                label = "Add passive provider",
                method = () =>
                    Manager.AddInterface( new LogisticProviderPassive() ),
                mode = DebugMenuOptionMode.Action,
            };
        }

        private static DebugMenuOption Option_EditPP ( LogisticProviderPassive providerPassive )
        {
            return new DebugMenuOption
            {
                label = "Edit provider " + providerPassive.GetHashCode(),
                method = () =>
                    Find.WindowStack.Add( new Dialog_DebugOptionListLister( Options_EditPP( providerPassive ) ) ),
                mode = DebugMenuOptionMode.Action,
            };
        }

        private static IEnumerable<DebugMenuOption> Options_EditPP ( LogisticProviderPassive providerPassive )
        {
            yield return Option_Remove( providerPassive );
            yield return new DebugMenuOption
            {
                label = "Add filter",
                method = () =>
                    Find.WindowStack.Add( new Dialog_DebugOptionListLister( Options_EditPP_AddFilter( providerPassive ) ) ),
                mode = DebugMenuOptionMode.Action,
            };
        }

        private static IEnumerable<DebugMenuOption> Options_EditPP_AddFilter ( LogisticProviderPassive providerPassive )
        {
            foreach ( var item in DefDatabase<ThingDef>.AllDefs )
            {
                yield return new DebugMenuOption
                {
                    label = item.label,
                    method = () =>
                        providerPassive.thingFilter.SetAllow( item, true ),
                    mode = DebugMenuOptionMode.Action,
                };
            }
        }

        //// requester

        //private static DebugMenuOption Option_AddReq ()
        //{
        //    return new DebugMenuOption
        //    {
        //        label = "Add requester",
        //        method = () =>
        //        {
        //            Find.WindowStack.Add( new Dialog_DebugOptionListLister( Option_AddReq_ChooseDef() ) );
        //        },
        //        mode = DebugMenuOptionMode.Action,
        //    };
        //}

        //private static IEnumerable<DebugMenuOption> Option_AddReq_ChooseDef ()
        //{
        //    foreach ( var item in DefDatabase<ThingDef>.AllDefs )
        //    {
        //        yield return new DebugMenuOption
        //        {
        //            label = item.label,
        //            method = () =>
        //            {
        //                var req = new LogisticRequester( item );

        //                Find.WindowStack.Add(
        //                    new Dialog_Slider(
        //                        n => req.ThingDef.defName + ": " + n,
        //                        0, 1500,
        //                        n => req.Count = n,
        //                        req.Count ) );

        //                Manager.AddInterface( req );
        //            },
        //            mode = DebugMenuOptionMode.Action,
        //        };
        //    }
        //}

        //private static DebugMenuOption Option_EditReq ( LogisticRequester requester )
        //{
        //    return new DebugMenuOption
        //    {
        //        label = "Edit requester " + requester.ThingDef.defName,
        //        method = () =>
        //            Find.WindowStack.Add( new Dialog_DebugOptionListLister( Options_EditReq( requester ) ) ),
        //        mode = DebugMenuOptionMode.Action,
        //    };
        //}

        //private static IEnumerable<DebugMenuOption> Options_EditReq ( LogisticRequester requester )
        //{
        //    yield return Option_Remove( requester );
        //    yield return new DebugMenuOption
        //    {
        //        label = "Edit count",
        //        method = () =>
        //            Find.WindowStack.Add(
        //                new Dialog_Slider(
        //                    n => requester.ThingDef.defName + ": " + n,
        //                    0, 1500,
        //                    n => requester.Count = n,
        //                    requester.Count ) ),
        //        mode = DebugMenuOptionMode.Action,
        //    };
        //}
    }
}