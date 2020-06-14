using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LogisticRim
{
    internal class LogisticChannel : IExposable, ILoadReferenceable
    {
        public LogisticChannel ( string name )
        {
            this.name = name;
            AllChannels.Add( this );
        }

        public LogisticChannel ()
        {
        }

        public string name;

        public List<LogisticInterface> interfaces = new List<LogisticInterface>();

        public IEnumerable<LogisticManager> Managers
        {
            get =>
                from i in interfaces
                group i by i.manager into g
                select g.Key;
        }

        public IEnumerable<LogisticProviderPassive> PassiveProviders
        {
            get =>
                interfaces.Select( i => i as LogisticProviderPassive )
                .Where( p => p != null );
        }

        public IEnumerable<LogisticRequester> Requesters
        {
            get =>
                interfaces.Select( i => i as LogisticRequester )
                .Where( p => p != null );
        }

        public IEnumerable<Thing> ThingsProvided
        {
            get =>
                from m in Managers
                from c in m.ThingsProvided( this )
                select c;
        }

        public static HashSet<LogisticChannel> AllChannels => Current.Game.GetComponent<LogisticChannels>().channels;

        public IEnumerable<Shipment> ActiveShipments
        {
            get =>
                this.Managers.SelectMany( m => m.shipmentsLoading )
                .Concat( this.Managers.SelectMany( m => m.shipmentsPlanned ) )
                .Concat( this.Managers.SelectMany( m => m.shipmentsReady ) );
        }

        public string GetUniqueLoadID ()
        {
            return "LogisticChannel_" + name;
        }

        public void ExposeData ()
        {
            Scribe_Values.Look( ref name, "name" );
            Scribe_Collections.Look( ref interfaces, "interfaces", LookMode.Reference );

            if ( Scribe.mode == LoadSaveMode.PostLoadInit )
            {
                if ( interfaces.Any( i => i == null ) )
                {
                    Log.Error( "[LogisticRim] A channel has some null interface." );
                }
            }
        }
    }
}