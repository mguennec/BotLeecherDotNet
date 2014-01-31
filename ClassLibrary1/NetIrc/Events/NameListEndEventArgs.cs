// Type: NetIrc2.Events.NameListEndEventArgs
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

using BotLeecher.NetIrc.Details;
using System;

namespace BotLeecher.NetIrc.Events
{
    /// <summary>
    /// Marks the end of a channel's name list.
    /// 
    /// </summary>
    public class NameListEndEventArgs : EventArgs
    {
        /// <summary>
        /// The channel the name list has been sent for.
        /// 
        /// </summary>
        public IrcString Channel { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="T:NetIrc2.Events.NameListEndEventArgs"/>.
        /// 
        /// </summary>
        /// <param name="channel">The channel the name list has been sent for.</param>
        public NameListEndEventArgs(IrcString channel)
        {
            ThrowExtensions.Null<IrcString>(Throw.If, channel, "channel");
            this.Channel = channel;
        }
    }
}
