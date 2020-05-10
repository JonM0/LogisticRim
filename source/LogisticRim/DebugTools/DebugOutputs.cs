using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LogisticRim.DebugTools
{
    internal class DebugOutputs
    {
        private static LogisticManager Manager => Find.CurrentMap.GetComponent<LogisticManager>();

        [DebugOutput( "Logistics", true )]
        public static void ProvidedItems ()
        {
            IEnumerable<Thing> dataSources =
                Manager.Channels
                .SelectMany( c => Manager.ThingsProvided( c ) );

            TableDataGetter<Thing>[] array = new TableDataGetter<Thing>[2];
            array[0] = new TableDataGetter<Thing>( "thing", ( Thing d ) => d.def.defName );
            array[1] = new TableDataGetter<Thing>( "quantity", ( Thing d ) => d.stackCount );

            DebugTables.MakeTablesDialog( dataSources, array );
        }

        [DebugOutput( "Logistics", true )]
        public static void ListChannels ()
        {
            IEnumerable<LogisticChannel> dataSources = LogisticChannel.AllChannels;

            TableDataGetter<LogisticChannel>[] array = new TableDataGetter<LogisticChannel>[4];
            array[0] = new TableDataGetter<LogisticChannel>( "name", ( LogisticChannel c ) => c.name );
            array[1] = new TableDataGetter<LogisticChannel>( "interface count", ( LogisticChannel c ) => c.interfaces.Count );
            array[2] = new TableDataGetter<LogisticChannel>( "providers", ( LogisticChannel c ) => c.PassiveProviders.Count() );
            array[3] = new TableDataGetter<LogisticChannel>( "requesters", ( LogisticChannel c ) => c.Requesters.Count() );

            DebugTables.MakeTablesDialog( dataSources, array );
        }

        [DebugOutput( "Logistics", true )]
        public static void RequestedItems ()
        {
            IEnumerable<LogisticRequester> dataSources =
                Manager.Requesters;

            TableDataGetter<LogisticRequester>[] array = new TableDataGetter<LogisticRequester>[3];
            array[0] = new TableDataGetter<LogisticRequester>( "thingDef", ( LogisticRequester r ) => r.ThingDef.defName );
            array[1] = new TableDataGetter<LogisticRequester>( "amount", ( LogisticRequester r ) => r.Count );
            array[2] = new TableDataGetter<LogisticRequester>( "missing", ( LogisticRequester r ) => r.Missing );

            DebugTables.MakeTablesDialog( dataSources, array );
        }
    }
}