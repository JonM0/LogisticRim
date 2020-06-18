using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;
using RimWorld;

namespace LogisticRim
{
    public static class LogTransporterUtility
    {
        public static List<CompLogisticTransporter> GetAvailableGroup ( Map map )
        {
            return null;
        }

        public static void DistributeItems ( IEnumerable<TransferableOneWay> items, List<CompTransporter> transporters )
        {
            // code adapted from RimWorld.Dialog_LoadTransporters.AssignTransferablesToRandomTransporters

            Dictionary<TransferableOneWay, int> tmpLeftCountToTransfer = new Dictionary<TransferableOneWay, int>();

            foreach ( var item in items )
            {
                tmpLeftCountToTransfer.Add( item, item.CountToTransfer );
            }

            TransferableOneWay biggestTransferable = items.MaxBy( ( TransferableOneWay x ) => tmpLeftCountToTransfer[x] );

            int transporterIndex = 0;
            // load all but the biggest
            foreach ( var transferable in items )
            {
                if ( transferable != biggestTransferable && tmpLeftCountToTransfer[transferable] > 0 )
                {
                    transporters[transporterIndex % transporters.Count].AddToTheToLoadList( transferable, tmpLeftCountToTransfer[transferable] );
                    transporterIndex++;
                }
            }
            // if there are empty pods distribute the biggest among the remaining
            if ( transporterIndex < transporters.Count )
            {
                int amountToDistribute = tmpLeftCountToTransfer[biggestTransferable];
                int amountEach = amountToDistribute / (transporters.Count - transporterIndex);
                for ( int m = transporterIndex; m < transporters.Count; m++ )
                {
                    int amountAdded = (m == transporters.Count - 1) ? amountToDistribute : amountEach; // on the last one add all the remaining
                    if ( amountAdded > 0 )
                    {
                        transporters[m].AddToTheToLoadList( biggestTransferable, amountAdded );
                    }
                    amountToDistribute -= amountAdded;
                }
            }
            // else just add it to one
            else
            {
                transporters[transporterIndex % transporters.Count].AddToTheToLoadList( biggestTransferable, tmpLeftCountToTransfer[biggestTransferable] );
            }
        }
    }
}