// Type: NetIrc2.Events.ChannelListEntryEventArgs
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

using BotLeecher.NetIrc.Details;
using System;

namespace BotLeecher.NetIrc.Events
{
    /// <summary>
    /// Stores an entry of the channel list.
    /// 
    /// </summary>
    public class ChannelListEntryEventArgs : EventArgs
    {
        /// <summary>
        /// The channel name.
        /// 
        /// </summary>
        public IrcString Channel { get; private set; }

        /// <summary>
        /// The number of users in the channel.
        /// 
        /// </summary>
        public int UserCount { get; private set; }

        /// <summary>
        /// The channel topic.
        /// 
        /// </summary>
        public IrcString Topic { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="T:NetIrc2.Events.ChannelListEntryEventArgs"/>.
        /// 
        /// </summary>
        /// <param name="channel">The channel name.</param><param name="userCount">The number of users in the channel.</param><param name="topic">The channel topic.</param>
        public ChannelListEntryEventArgs(IrcString channel, int userCount, IrcString topic)
        {
            ThrowExtensions.Null<IrcString>(ThrowExtensions.Negative(ThrowExtensions.Null<IrcString>(Throw.If, channel, "channel"), userCount, "userCount"), topic, "topic");
            this.Channel = channel;
            this.UserCount = userCount;
            this.Topic = topic;
        }
    }
}
