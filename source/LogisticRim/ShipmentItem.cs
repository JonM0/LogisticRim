using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LogisticRim
{
    internal class ShipmentItem : IExposable
    {
        public LogisticRequester requester;
        public LogisticManager sender;

        private int count = 0;
        public HashSet<Thing> things = new HashSet<Thing>();
        public List<ThingCountClass> thingCounts = new List<ThingCountClass>();

        public int Count => this.count;

        public bool InsertThing ( Thing thing )
        {
            if ( requester.ThingDef != thing.def )
            {
                Log.Error( "[LogisticRim] Tried to add " + thing.Label + " to shipment of " + requester.ThingDef.defName );
                return false;
            }
            else
            {
                if ( requester.Missing - count > 0 && things.Add( thing ) )
                {
                    int take = Math.Min( thing.stackCount, requester.Missing - count );
                    count += take;
                    thingCounts.Add( new ThingCountClass( thing, take ) );

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void Execute ( List<ActiveDropPodInfo> dropPodList )
        {
            ActiveDropPodInfo pod = dropPodList.LastOrDefault();

            foreach ( var item in thingCounts )
            {
                int count = item.Count;

                while ( count > 0 )
                {
                    if ( pod == null )
                    {
                        pod = new ActiveDropPodInfo();
                        dropPodList.Add( pod );
                    }

                    int canAccept = pod.innerContainer.GetCountCanAccept( item.thing );

                    canAccept = Math.Min( canAccept, count );

                    if ( canAccept == 0 )
                    {
                        ActiveDropPodInfo newPod = new ActiveDropPodInfo();

                        canAccept = newPod.innerContainer.GetCountCanAccept( item.thing );
                        canAccept = Math.Min( canAccept, count );

                        if ( canAccept == 0 )
                            break;
                        else
                            pod = newPod;
                    }

                    bool success = pod.innerContainer.TryAdd( item.thing.SplitOff( canAccept ) );

                    if ( success )
                    {
                        Log.Message( item.thing.Label + " " + count );
                        count -= canAccept;
                    }
                }
            }
        }

        public void ExposeData ()
        {
            Scribe_References.Look( ref requester, "requester" );
            Scribe_References.Look( ref sender, "sender" );
            Scribe_Values.Look( ref count, "count" );

            Scribe_Collections.Look( ref things, "things", LookMode.Reference );
            Scribe_Collections.Look( ref thingCounts, "thingCounts", LookMode.Deep );
        }

        public ShipmentItem ( LogisticRequester requester, LogisticManager sender )
        {
            this.requester = requester;
            this.sender = sender;
        }

        public bool Empty => this.Count == 0;
    }
}