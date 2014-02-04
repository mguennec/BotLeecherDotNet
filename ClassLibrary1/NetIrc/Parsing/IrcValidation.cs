// Type: NetIrc2.Parsing.IrcValidation
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace BotLeecher.NetIrc.Parsing
{
    /// <summary>
    /// Validates various parameter types.
    /// 
    /// </summary>
    public static class IrcValidation
    {
        /// <summary>
        /// Checks if the channel name is valid. The definition used by this test is somewhat loose.
        /// 
        ///             Channel names may not contain spaces, commas, NULL, BELL, CR, or LF, and must start with # or &amp;.
        /// 
        /// </summary>
        /// <param name="channel">The channel name to test.</param>
        /// <returns>
        /// <c>true</c> if the name is valid.
        /// </returns>
        public static bool IsChannelName(IrcString channel)
        {
            if (channel == (IrcString)null)
            {
                return false;
            }
            else
            {
                string errorMessage;
                return IrcValidation.ValidateChannel(channel, out errorMessage);
            }
        }

        /// <summary>
        /// Checks if the nickname is valid. The definition used by this test is somewhat loose.
        /// 
        ///             Nicknames may not contain spaces, commas, NULL, BELL, CR, LF, #, &amp;, @, or +.
        /// 
        /// </summary>
        /// <param name="nickname">The nickname to test.</param>
        /// <returns>
        /// <c>true</c> if the name is valid.
        /// </returns>
        public static bool IsNickname(IrcString nickname)
        {
            if (nickname == (IrcString)null)
            {
                return false;
            }
            else
            {
                string errorMessage;
                return IrcValidation.ValidateNickname(nickname, out errorMessage);
            }
        }

        internal static bool ValidateChannel(IrcString channel, out string errorMessage)
        {
            if (!IrcValidation.ValidateTarget(channel, out errorMessage))
                return false;
            if ((int)channel[0] == 35 || (int)channel[0] == 38)
                return true;
            errorMessage = "Channel names must begin with # or &.";
            return false;
        }

        internal static bool ValidateNickname(IrcString nickname, out string errorMessage)
        {
            if (!IrcValidation.ValidateTarget(nickname, out errorMessage))
                return false;
            if (!nickname.Contains((byte)35) && !nickname.Contains((byte)38) && (!nickname.Contains((byte)64) && !nickname.Contains((byte)43)))
                return true;
            errorMessage = "Nicknames may not contain #, &, @, or +.";
            return false;
        }

        internal static bool ValidateTarget(IrcString target, out string errorMessage)
        {
            if (!IrcValidation.ValidateParameter(target, false, out errorMessage))
                return false;
            if (target.Length == 0)
            {
                errorMessage = "Targets may not be zero-byte.";
                return false;
            }
            else
            {
                if (!target.Contains((byte)7) && !target.Contains((byte)44))
                    return true;
                errorMessage = "Targets may not contain BELL or a comma.";
                return false;
            }
        }

        internal static bool ValidateParameter(IrcString parameter, bool trailing, out string errorMessage)
        {
            errorMessage = (string)null;
            if (!trailing && (parameter.Contains((byte)32) || parameter.StartsWith((IrcString)":")))
            {
                errorMessage = "Only the trailing parameter may contain spaces or start with a colon.";
                return false;
            }
            else
            {
                if (!parameter.Contains((byte)0) && !parameter.Contains((byte)13) && !parameter.Contains((byte)10))
                    return true;
                errorMessage = "IRC does not allow embedded NULL, CR, or LF.";
                return false;
            }
        }

        internal static bool ValidateIdentityPart(IrcString identityPart, out string errorMessage)
        {
            if (!IrcValidation.ValidateParameter(identityPart, false, out errorMessage))
                return false;
            if (!identityPart.Contains((byte)64) && !identityPart.Contains((byte)33))
                return true;
            errorMessage = "Identity parts may not contain @ or !.";
            return false;
        }

        internal static bool ValidateIdentity(IrcIdentity identity, out string errorMessage)
        {
            errorMessage = (string)null;
            if (identity.Nickname == (IrcString)null || identity.Nickname.Length == 0)
            {
                errorMessage = "Nickname is not set.";
                return false;
            }
            else
                return IrcValidation.ValidateIdentityPart(identity.Nickname, out errorMessage) && (!(identity.Username != (IrcString)null) || IrcValidation.ValidateIdentityPart(identity.Username, out errorMessage)) && (!(identity.Hostname != (IrcString)null) || IrcValidation.ValidateIdentityPart(identity.Hostname, out errorMessage));
        }

        internal static bool ValidateStatement(IrcStatement statement, out string errorMessage)
        {
            errorMessage = (string)null;
            if (statement.Source != (IrcIdentity)null && !IrcValidation.ValidateIdentity(statement.Source, out errorMessage))
                return false;
            if (statement.Command == (IrcString)null)
            {
                errorMessage = "Command is not set.";
                return false;
            }
            else if ((statement.Command.Length != 3 || !Enumerable.All<byte>((IEnumerable<byte>)statement.Command, (Func<byte, bool>)(x =>
            {
                if ((int)x >= 48)
                    return (int)x <= 57;
                else
                    return false;
            }))) && (statement.Command.Length <= 0 || !Enumerable.All<byte>((IEnumerable<byte>)statement.Command, (Func<byte, bool>)(x =>
            {
                if ((int)x >= 65)
                    return (int)x <= 90;
                else
                    return false;
            }))))
            {
                errorMessage = "Command is invalid.";
                return false;
            }
            else if (statement.Parameters.Count > 15)
            {
                errorMessage = string.Format("IRC only allows up to {0} parameters.", (object)15);
                return false;
            }
            else
            {
                for (int index = 0; index < statement.Parameters.Count; ++index)
                {
                    IrcString parameter = statement.Parameters[index];
                    if (parameter == (IrcString)null)
                    {
                        errorMessage = "No parameters may be null.";
                        return false;
                    }
                    else if (!IrcValidation.ValidateParameter(parameter, index == statement.Parameters.Count - 1, out errorMessage))
                        return false;
                }
                return true;
            }
        }
    }
}
