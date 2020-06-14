using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LogisticRim
{
    internal abstract class LogisticInterface : IExposable, ILoadReferenceable
    {
        protected LogisticInterface ( LogisticChannel channel )
        {
            this.channel = channel;
            this.channel.interfaces.Add( this );
        }

        protected LogisticInterface ()
        {
            this.channel = LogisticChannel.AllChannels.First();
            this.channel.interfaces.Add( this );
        }

        public LogisticManager manager;

        public LogisticChannel channel;

        public void Remove ()
        {
            manager.interfaces.Remove( this );
            manager = null;
            channel.interfaces.Remove( this );
            channel = null;
        }

        virtual public void ExposeData ()
        {
            Scribe_References.Look( ref this.manager, "manager" );
            Scribe_References.Look( ref this.channel, "channel" );
            Scribe_Values.Look( ref loadid, "loadid" );
        }

        public string GetUniqueLoadID ()
        {
            return "Interface_" + loadid;
        }

        internal string loadid;
    }
}