// Type: NetIrc2.IrcIdentity
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

using BotLeecher.NetIrc.Parsing;
using System;

namespace BotLeecher.NetIrc
{
    /// <summary>
    /// Stores an IRC user's identity information - their nickname, username, and hostname.
    /// 
    /// </summary>
    public class IrcIdentity
    {
        /// <summary>
        /// The user's IRC nickname (the name shown in channels).
        /// 
        /// </summary>
        public IrcString Nickname { get; set; }

        /// <summary>
        /// The username.
        /// 
        /// </summary>
        public IrcString Username { get; set; }

        /// <summary>
        /// The user's hostname.
        /// 
        /// </summary>
        public IrcString Hostname { get; set; }

        /// <summary>
        /// Converts an IRC identity into an IRC string.
        /// 
        /// </summary>
        /// <param name="identity">The IRC identity.</param>
        /// <returns>
        /// The IRC string.
        /// </returns>
        public static implicit operator IrcString(IrcIdentity identity)
        {
            if (!(identity != (IrcIdentity)null))
                return (IrcString)null;
            else
                return identity.ToIrcString();
        }

        /// <summary>
        /// Compares two identities for equality.
        /// 
        /// </summary>
        /// <param name="identity1">The first identity.</param><param name="identity2">The second identity.</param>
        /// <returns>
        /// <c>true</c> if the identities are equal.
        /// </returns>
        public static bool operator ==(IrcIdentity identity1, IrcIdentity identity2)
        {
            return object.Equals((object)identity1, (object)identity2);
        }

        /// <summary>
        /// Compares two identities for inequality.
        /// 
        /// </summary>
        /// <param name="identity1">The first identity.</param><param name="identity2">The second identity.</param>
        /// <returns>
        /// <c>true</c> if the identities are not equal.
        /// </returns>
        public static bool operator !=(IrcIdentity identity1, IrcIdentity identity2)
        {
            return !object.Equals((object)identity1, (object)identity2);
        }

        /// <summary>
        /// Tries to parse a string to get an IRC identity.
        /// 
        ///             IRC identities are formatted as nickname!username@hostname.
        /// 
        /// </summary>
        /// <param name="string">The string to parse.</param><param name="identity">The identity, or <c>null</c> if parsing fails.</param>
        /// <returns>
        /// <c>true</c> if parsing completed successfully.
        /// </returns>
        public static bool TryParse(IrcString @string, out IrcIdentity identity)
        {
            identity = (IrcIdentity)null;
            if (!(@string == (IrcString)null))
            {
                identity = new IrcIdentity();
                IrcString[] ircStringArray1 = @string.Split((byte)64);
                if (ircStringArray1.Length >= 2)
                {
                    identity.Hostname = ircStringArray1[1];
                    @string = ircStringArray1[0];
                }
                IrcString[] ircStringArray2 = @string.Split((byte)33);
                if (ircStringArray2.Length >= 2)
                {
                    identity.Username = ircStringArray2[1];
                    @string = ircStringArray2[0];
                }
                identity.Nickname = ircStringArray2[0];
                string errorMessage;
                if (IrcValidation.ValidateIdentity(identity, out errorMessage))
                    return true;
            }
            identity = (IrcIdentity)null;
            return false;
        }

        /// <summary>
        /// Converts an IRC identity into an IRC string.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// The IRC string.
        /// </returns>
        public IrcString ToIrcString()
        {
            string errorMessage;
            if (!IrcValidation.ValidateIdentity(this, out errorMessage))
                throw new InvalidOperationException(errorMessage);
            IrcString nickname = this.Nickname;
            if (this.Username != (IrcString)null)
                nickname += (IrcString)"!" + this.Username;
            if (this.Hostname != (IrcString)null)
                nickname += (IrcString)"@" + this.Hostname;
            return nickname;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            IrcIdentity ircIdentity = obj as IrcIdentity;
            if (ircIdentity != (IrcIdentity)null && object.Equals((object)this.Nickname, (object)ircIdentity.Nickname) && object.Equals((object)this.Username, (object)ircIdentity.Username))
                return object.Equals((object)this.Hostname, (object)ircIdentity.Hostname);
            else
                return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return (this.Nickname ?? IrcString.Empty).GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return (string)this.ToIrcString();
        }
    }
}
