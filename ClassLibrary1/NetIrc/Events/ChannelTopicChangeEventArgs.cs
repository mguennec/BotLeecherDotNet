// Type: NetIrc2.Events.ChannelTopicChangeEventArgs
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

using BotLeecher.NetIrc.Details;
using System;

namespace BotLeecher.NetIrc.Events
{
    /// <summary>
    /// Stores a change in a channel's topic.
    /// 
    /// </summary>
    public class ChannelTopicChangeEventArgs : EventArgs
    {
        /// <summary>
        /// The channel name.
        /// 
        /// </summary>
        public IrcString Channel { get; private set; }

        /// <summary>
        /// The new channel topic.
        /// 
        /// </summary>
        public IrcString NewTopic { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="T:NetIrc2.Events.ChannelTopicChangeEventArgs"/>.
        /// 
        /// </summary>
        /// <param name="channel">The channel name.</param><param name="newTopic">The new channel topic.</param>
        public ChannelTopicChangeEventArgs(IrcString channel, IrcString newTopic)
        {
            ThrowExtensions.Null<IrcString>(ThrowExtensions.Null<IrcString>(Throw.If, channel, "channel"), newTopic, "newTopic");
            this.Channel = channel;
            this.NewTopic = newTopic;
        }
    }
}
