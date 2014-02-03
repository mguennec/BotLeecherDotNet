// Type: NetIrc2.Events.ChatMessageEventArgs
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll


using BotLeecher.NetIrc.Details;
namespace BotLeecher.NetIrc.Events
{
    /// <summary>
    /// Stores a complete chat message.
    /// 
    /// </summary>
    public class ChatMessageEventArgs : TargetedMessageEventArgs
    {
        /// <summary>
        /// The chat message.
        /// 
        /// </summary>
        public IrcString Message { get; private set; }

        /// <summary>
        /// Creates an instance of <see cref="T:NetIrc2.Events.ChatMessageEventArgs"/>.
        /// 
        /// </summary>
        /// <param name="sender">The sender, or <c>null</c> if the message has no sender.</param><param name="recipient">The recipient.</param><param name="message">The chat message.</param>
        public ChatMessageEventArgs(IrcIdentity sender, IrcString recipient, IrcString message)
            : base(sender, recipient)
        {
            ThrowExtensions.Null<IrcString>(Throw.If, message, "message");
            this.Message = message;
        }
    }
}
