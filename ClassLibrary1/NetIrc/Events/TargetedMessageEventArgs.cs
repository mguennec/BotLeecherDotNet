// Type: NetIrc2.Events.TargetedMessageEventArgs
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

using BotLeecher.NetIrc.Details;
using System;

namespace BotLeecher.NetIrc.Events
{
    /// <summary>
    /// Stores a sender and recipient for a targeted action.
    /// 
    /// </summary>
    public class TargetedMessageEventArgs : EventArgs
    {
        /// <summary>
        /// The sender.
        /// 
        ///             Be aware that some messages may not have a sender, such as NOTICEs from
        ///             the server at connect time. In this case the sender will be <c>null</c>.
        /// 
        /// </summary>
        public IrcIdentity Sender { get; private set; }

        /// <summary>
        /// The recipient.
        /// 
        /// </summary>
        public IrcString Recipient { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="T:NetIrc2.Events.TargetedMessageEventArgs"/>.
        /// 
        /// </summary>
        /// <param name="sender">The sender, or <c>null</c> if the message has no sender.</param><param name="recipient">The recipient.</param>
        public TargetedMessageEventArgs(IrcIdentity sender, IrcString recipient)
        {
            ThrowExtensions.Null<IrcString>(Throw.If, recipient, "recipient");
            this.Sender = sender;
            this.Recipient = recipient;
        }
    }
}
