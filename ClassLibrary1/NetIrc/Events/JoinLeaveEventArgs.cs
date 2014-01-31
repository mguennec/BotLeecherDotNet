// Type: NetIrc2.Events.JoinLeaveEventArgs
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

using BotLeecher.NetIrc.Details;
using System;
using System.Collections.Generic;

namespace BotLeecher.NetIrc.Events
{
    /// <summary>
    /// Stores information about a user joining or leaving a channel.
    /// 
    /// </summary>
    public class JoinLeaveEventArgs : EventArgs
    {
        private IrcString[] _channels;

        /// <summary>
        /// The user who joined or left the channel(s).
        /// 
        /// </summary>
        public IrcIdentity Identity { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="T:NetIrc2.Events.JoinLeaveEventArgs"/>.
        /// 
        /// </summary>
        /// <param name="identity">The user who joined or left the channel(s).</param><param name="channels">The list of channels joined or left.</param>
        public JoinLeaveEventArgs(IrcIdentity identity, IrcString[] channels)
        {
            ThrowExtensions.NullElements<IrcString>(ThrowExtensions.Null<IrcIdentity>(Throw.If, identity, "identity"), (IEnumerable<IrcString>)channels, "channels");
            this.Identity = identity;
            this._channels = (IrcString[])channels.Clone();
        }

        /// <summary>
        /// Gets the list of channels joined or left.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// An array of channel names.
        /// </returns>
        public IrcString[] GetChannelList()
        {
            return (IrcString[])this._channels.Clone();
        }
    }
}
