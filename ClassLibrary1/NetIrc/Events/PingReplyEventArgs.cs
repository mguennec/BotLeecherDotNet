// Type: NetIrc2.Events.PingReplyEventArgs
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

using BotLeecher.NetIrc.Details;
using System;

namespace BotLeecher.NetIrc.Events
{
    /// <summary>
    /// Stores the results of an earlier ping request.
    /// 
    /// </summary>
    public class PingReplyEventArgs : EventArgs
    {
        /// <summary>
        /// The sender of the reply.
        /// 
        /// </summary>
        public IrcIdentity Identity { get; private set; }

        /// <summary>
        /// The ping time, in milliseconds.
        /// 
        /// </summary>
        public int Delay { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="T:NetIrc2.Events.PingReplyEventArgs"/>.
        /// 
        /// </summary>
        /// <param name="identity">The user who is replying to your ping request.</param><param name="delay">The ping time, in milliseconds.</param>
        public PingReplyEventArgs(IrcIdentity identity, int delay)
        {
            ThrowExtensions.Negative(ThrowExtensions.Null<IrcIdentity>(Throw.If, identity, "identity"), delay, "delay");
            this.Identity = identity;
            this.Delay = delay;
        }
    }
}
