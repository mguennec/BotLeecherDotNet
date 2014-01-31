// Type: NetIrc2.Events.IrcErrorEventArgs
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

using BotLeecher.NetIrc.Details;
using BotLeecher.NetIrc.Parsing;
using System;

namespace BotLeecher.NetIrc.Events
{
    /// <summary>
    /// Stores an error.
    /// 
    /// </summary>
    public class IrcErrorEventArgs : EventArgs
    {
        /// <summary>
        /// The type of error that has occured.
        /// 
        /// </summary>
        public IrcReplyCode Error { get; private set; }

        /// <summary>
        /// The raw IRC statement data. This can be used to garner more information about the error.
        /// 
        /// </summary>
        public IrcStatement Data { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="T:NetIrc2.Events.IrcErrorEventArgs"/>.
        /// 
        /// </summary>
        /// <param name="error">The type of error that has occured.</param><param name="data">The raw IRC statement data. This can be used to garner more information about the error.</param>
        public IrcErrorEventArgs(IrcReplyCode error, IrcStatement data)
        {
            ThrowExtensions.Null<IrcStatement>(ThrowExtensions.Null<IrcReplyCode>(Throw.If, error, "error"), data, "data");
            this.Error = error;
            this.Data = data;
        }
    }
}
