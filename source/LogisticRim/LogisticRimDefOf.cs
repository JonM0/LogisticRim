using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;
using RimWorld;
using System.ComponentModel;

namespace LogisticRim
{
    [DefOf]
    internal static class LogisticRimDefOf
    {
        static LogisticRimDefOf ()
        {
            DefOfHelper.EnsureInitializedInCtor( typeof( LogisticRimDefOf ) );
        }

        //public static readonly TableDef<T> Requesters;
    }
}