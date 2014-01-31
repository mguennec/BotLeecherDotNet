// Type: NetIrc2.Events.ModeEventArgs
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

using BotLeecher.NetIrc.Details;
using System.Collections.Generic;

namespace BotLeecher.NetIrc.Events
{
    /// <summary>
    /// Stores a mode change.
    /// 
    /// </summary>
    public class ModeEventArgs : TargetedMessageEventArgs
    {
        private IrcString[] _parameters;

        /// <summary>
        /// The mode change, for example +o or +v.
        /// 
        /// </summary>
        public IrcString Command { get; private set; }

        /// <summary>
        /// The number of parameters.
        /// 
        /// </summary>
        public int ParameterCount
        {
            get
            {
                return this._parameters.Length;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="T:NetIrc2.Events.ModeEventArgs"/>.
        /// 
        /// </summary>
        /// <param name="sender">The user changing the mode.</param><param name="recipient">The target of the mode change. This may be a channel or a user.</param><param name="command">The mode change, for example +o or +v.</param><param name="parameters">The mode change parameters.</param>
        public ModeEventArgs(IrcIdentity sender, IrcString recipient, IrcString command, IrcString[] parameters)
            : base(sender, recipient)
        {
            ThrowExtensions.NullElements<IrcString>(ThrowExtensions.Null<IrcString>(Throw.If, command, "command"), (IEnumerable<IrcString>)parameters, "parameters");
            this.Command = command;
            this._parameters = (IrcString[])parameters.Clone();
        }

        /// <summary>
        /// Gets a mode change parameter.
        /// 
        /// </summary>
        /// <param name="index">The index of the parameter.</param>
        /// <returns>
        /// A parameter.
        /// </returns>
        public IrcString GetParameter(int index)
        {
            return this._parameters[index];
        }

        /// <summary>
        /// Gets all of the mode change parameters.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// An array of parameters.
        /// </returns>
        public IrcString[] GetParameterList()
        {
            return (IrcString[])this._parameters.Clone();
        }
    }
}
