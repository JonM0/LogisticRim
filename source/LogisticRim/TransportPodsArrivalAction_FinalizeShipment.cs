using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;
using RimWorld;
using RimWorld.Planet;

namespace LogisticRim
{
    internal class TransportPodsArrivalAction_FinalizeShipment : TransportPodsArrivalAction
    {
        public override void Arrived ( List<ActiveDropPodInfo> pods, int tile )
        {
            // drop pods on trade drop spots

            Map map = this.shipment.destination.map;
            IntVec3 dropCenter = DropCellFinder.TradeDropSpot( map );

            foreach ( var pod in pods )
            {
                if ( DropCellFinder.TryFindDropSpotNear( dropCenter, shipment.destination.map, out IntVec3 spot, false, false, false, null ) )
                {
                    DropPodUtility.MakeDropPodAt( spot, map, pod );
                }
                else
                {
                    DropPodUtility.MakeDropPodAt( dropCenter, map, pod );
                }
            }

            // set shipment as complete

            this.shipment.Status = Shipment.ShipmentStatus.Complete;
        }

        public static bool CanLandInSpecificCell ( IEnumerable<IThingHolder> pods, MapParent mapParent )
        {
            return mapParent != null && mapParent.Spawned && mapParent.HasMap;
        }

        public Shipment shipment;

        public TransportPodsArrivalAction_FinalizeShipment ( Shipment shipment )
        {
            this.shipment = shipment;
        }
    }
}