// Type: NetIrc2.Events.InvitationEventArgs
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll


using BotLeecher.NetIrc.Details;
namespace BotLeecher.NetIrc.Events
{
    /// <summary>
    /// Stores information about a channel invitation.
    /// 
    /// </summary>
    public class InvitationEventArgs : TargetedMessageEventArgs
    {
        /// <summary>
        /// The channel the invitation is for.
        /// 
        /// </summary>
        public IrcString Channel { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="T:NetIrc2.Events.InvitationEventArgs"/>.
        /// 
        /// </summary>
        /// <param name="sender">The sender of the invitation.</param><param name="recipient">The recipient of the invitation.</param><param name="channel">The channel the invitation is for.</param>
        public InvitationEventArgs(IrcIdentity sender, IrcString recipient, IrcString channel)
            : base(sender, recipient)
        {
            ThrowExtensions.Null<IrcString>(Throw.If, channel, "channel");
            this.Channel = channel;
        }
    }
}
