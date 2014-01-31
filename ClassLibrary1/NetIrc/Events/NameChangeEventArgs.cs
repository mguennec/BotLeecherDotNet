// Type: NetIrc2.Events.NameChangeEventArgs
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

using BotLeecher.NetIrc.Details;
using System;

namespace BotLeecher.NetIrc.Events
{
    /// <summary>
    /// Stores information about a name change.
    /// 
    /// </summary>
    public class NameChangeEventArgs : EventArgs
    {
        /// <summary>
        /// The user who is changing their nickname.
        /// 
        /// </summary>
        public IrcIdentity Identity { get; private set; }

        /// <summary>
        /// The new nickname.
        /// 
        /// </summary>
        public IrcString NewName { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="T:NetIrc2.Events.NameChangeEventArgs"/>.
        /// 
        /// </summary>
        /// <param name="identity">The user who is changing their nickname.</param><param name="newName">The new nickname.</param>
        public NameChangeEventArgs(IrcIdentity identity, IrcString newName)
        {
            ThrowExtensions.Null<IrcString>(ThrowExtensions.Null<IrcIdentity>(Throw.If, identity, "identity"), newName, "newName");
            this.Identity = identity;
            this.NewName = newName;
        }
    }
}
