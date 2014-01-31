// Type: NetIrc2.Events.KickEventArgs
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll


using BotLeecher.NetIrc.Details;
namespace BotLeecher.NetIrc.Events
{
    /// <summary>
    /// Stores information about a user being kicked from a channel.
    /// 
    /// </summary>
    public class KickEventArgs : TargetedMessageEventArgs
    {
        /// <summary>
        /// The channel the user is being kicked from.
        /// 
        /// </summary>
        public IrcString Channel { get; private set; }

        /// <summary>
        /// The reason the user is being kicked, or <c>null</c> if none is given.
        /// 
        /// </summary>
        public IrcString Reason { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="T:NetIrc2.Events.KickEventArgs"/>.
        /// 
        /// </summary>
        /// <param name="sender">The user doing the kicking.</param><param name="recipient">The user being kicked out of the channel.</param><param name="channel">The channel the user is being kicked from.</param><param name="reason">The reason the user is being kicked, or <c>null</c> if none is given.</param>
        public KickEventArgs(IrcIdentity sender, IrcString recipient, IrcString channel, IrcString reason)
            : base(sender, recipient)
        {
            ThrowExtensions.Null<IrcString>(Throw.If, channel, "channel");
            this.Channel = channel;
            this.Reason = reason;
        }
    }
}
