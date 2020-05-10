using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;

namespace LogisticRim
{
    internal class LogisticChannels : GameComponent
    {
        public HashSet<LogisticChannel> channels = new HashSet<LogisticChannel>();

        public LogisticChannels ( Game game )
        {
        }

        public override void ExposeData ()
        {
            base.ExposeData();

            Scribe_Collections.Look<LogisticChannel>( ref channels, "channels", LookMode.Deep );
        }

        public override void StartedNewGame ()
        {
            base.StartedNewGame();

            new LogisticChannel( "0" );
        }
    }
}