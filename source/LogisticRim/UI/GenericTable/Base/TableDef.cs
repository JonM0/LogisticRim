using System;
using System.Collections.Generic;
using Verse;

namespace LogisticRim
{
    [StaticConstructorOnStartup]
    internal class TableDef<T>
    {
        public List<ColumnDef<T>> columns;

        public Type workerClass;

        public int minWidth = 998;
    }
}