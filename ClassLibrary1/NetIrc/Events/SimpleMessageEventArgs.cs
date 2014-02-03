// Type: NetIrc2.Events.SimpleMessageEventArgs
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

using BotLeecher.NetIrc.Details;
using System;

namespace BotLeecher.NetIrc.Events
{
    /// <summary>
    /// Stores a one-line message.
    /// 
    /// </summary>
    public class SimpleMessageEventArgs : EventArgs
    {
        /// <summary>
        /// The message.
        /// 
        /// </summary>
        public IrcString Message { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="T:NetIrc2.Events.SimpleMessageEventArgs"/>.
        /// 
        /// </summary>
        /// <param name="message">The message.</param>
        public SimpleMessageEventArgs(IrcString message)
        {
            ThrowExtensions.Null<IrcString>(Throw.If, message, "message");
            this.Message = message;
        }
    }
}
