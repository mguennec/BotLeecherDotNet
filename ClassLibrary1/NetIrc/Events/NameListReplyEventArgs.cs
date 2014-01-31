// Type: NetIrc2.Events.NameListReplyEventArgs
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

using BotLeecher.NetIrc.Details;
using System;
using System.Collections.Generic;

namespace BotLeecher.NetIrc.Events
{
    /// <summary>
    /// Stores a list of names of users in a channel.
    /// 
    /// </summary>
    public class NameListReplyEventArgs : EventArgs
    {
        private IrcString[] _names;

        /// <summary>
        /// The channel this list pertains to.
        /// 
        /// </summary>
        public IrcString Channel { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="T:NetIrc2.Events.NameListReplyEventArgs"/>.
        /// 
        /// </summary>
        /// <param name="channel">The channel this list pertains to.</param><param name="names">The names of users in the channel.</param>
        public NameListReplyEventArgs(IrcString channel, IrcString[] names)
        {
            ThrowExtensions.NullElements<IrcString>(ThrowExtensions.Null<IrcString>(Throw.If, channel, "channel"), (IEnumerable<IrcString>)names, "names");
            this.Channel = channel;
            this._names = (IrcString[])names.Clone();
        }

        /// <summary>
        /// Gets the list of names of users in the channel.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// An array of nicknames.
        /// </returns>
        public IrcString[] GetNameList()
        {
            return (IrcString[])this._names.Clone();
        }
    }
}
