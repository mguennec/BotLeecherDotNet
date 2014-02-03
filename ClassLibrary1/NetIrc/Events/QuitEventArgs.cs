// Type: NetIrc2.Events.QuitEventArgs
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

using BotLeecher.NetIrc.Details;
using System;

namespace BotLeecher.NetIrc.Events
{
    /// <summary>
    /// Stores information about a user disconnecting from an IRC server.
    /// 
    /// </summary>
    public class QuitEventArgs : EventArgs
    {
        /// <summary>
        /// The user who disconnected.
        /// 
        /// </summary>
        public IrcIdentity Identity { get; private set; }

        /// <summary>
        /// The quit message, or <c>null</c> if none was given.
        /// 
        /// </summary>
        public IrcString QuitMessage { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="T:NetIrc2.Events.QuitEventArgs"/>.
        /// 
        /// </summary>
        /// <param name="identity">The user who disconnected.</param><param name="quitMessage">The quit message, or <c>null</c> if none was given.</param>
        public QuitEventArgs(IrcIdentity identity, IrcString quitMessage)
        {
            ThrowExtensions.Null<IrcIdentity>(Throw.If, identity, "identity");
            this.Identity = identity;
            this.QuitMessage = quitMessage;
        }
    }
}
