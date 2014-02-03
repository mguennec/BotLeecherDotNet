// Type: NetIrc2.Parsing.IrcStatement
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

using BotLeecher.NetIrc.Details;
using System;
using System.Collections.Generic;

namespace BotLeecher.NetIrc.Parsing
{
    /// <summary>
    /// Reads and writes raw IRC statement lines.
    /// 
    /// </summary>
    public class IrcStatement
    {
        /// <summary>
        /// The source of the statement, if any. This is called the prefix in the IRC specification.
        /// 
        /// </summary>
        public IrcIdentity Source { get; set; }

        /// <summary>
        /// The command, or if the IRC statement is a reply, a three-digit number.
        /// 
        /// </summary>
        public IrcString Command { get; set; }

        /// <summary>
        /// The numeric reply code, if the IRC statement is a reply.
        /// 
        /// </summary>
        public IrcReplyCode ReplyCode
        {
            get
            {
                int result;
                if (!int.TryParse((string)this.Command, out result))
                    return (IrcReplyCode)0;
                else
                    return (IrcReplyCode)result;
            }
            set
            {
                this.Command = (IrcString)((int)value).ToString();
            }
        }

        /// <summary>
        /// The parameters of the statement.
        /// 
        /// </summary>
        public IList<IrcString> Parameters { get; private set; }

        /// <summary>
        /// Creates an IRC statement with nothing set.
        /// 
        /// </summary>
        public IrcStatement()
        {
            this.Parameters = (IList<IrcString>)new List<IrcString>();
        }

        /// <summary>
        /// Creates an IRC statement.
        /// 
        /// </summary>
        /// <param name="source">The source of the statement, if any. This is called the prefix in the IRC specification.</param><param name="command">The command or three-digit reply code.</param><param name="parameters">The parameters of the command.</param>
        public IrcStatement(IrcIdentity source, IrcString command, params IrcString[] parameters)
        {
            this.Source = source;
            this.Command = command;
            this.Parameters = (IList<IrcString>)new List<IrcString>((IEnumerable<IrcString>)parameters);
        }

        private static void SkipCrlf(byte[] buffer, ref int offset, ref int count)
        {
            while (count > 0 && ((int)buffer[offset] == 13 || (int)buffer[offset] == 10))
            {
                ++offset;
                --count;
            }
        }

        /// <summary>
        /// Tries to read a buffer and parse out an IRC statement.
        /// 
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param><param name="offset">The offset to begin reading. The parser may advance this, even if parsing fails.</param><param name="count">The maximum number of bytes to read.</param><param name="statement">The statement, if parsing succeeds, or <c>null</c>.</param>
        /// <returns>
        /// <c>true</c> if parsing succeeded.
        /// </returns>
        public static bool TryParse(byte[] buffer, ref int offset, int count, out IrcStatement statement)
        {
            IrcStatementParseResult parseResult;
            return IrcStatement.TryParse(buffer, ref offset, count, out statement, out parseResult);
        }

        /// <summary>
        /// Tries to read a buffer and parse out an IRC statement.
        ///             Additionally, on failure, the reason for failure is returned.
        /// 
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param><param name="offset">The offset to begin reading. The parser may advance this, even if parsing fails.</param><param name="count">The maximum number of bytes to read.</param><param name="statement">The statement, if parsing succeeds, or <c>null</c>.</param><param name="parseResult">The result of parsing. On failure, this is the reason for the failure.</param>
        /// <returns>
        /// <c>true</c> if parsing succeeded.
        /// </returns>
        public static bool TryParse(byte[] buffer, ref int offset, int count, out IrcStatement statement, out IrcStatementParseResult parseResult)
    {
      ThrowExtensions.Null<byte[]>(Throw.If, buffer, "buffer");
      string errorMessage = (string) null;
      statement = (IrcStatement) null;
      parseResult = IrcStatementParseResult.NothingToParse;
      IrcStatement.SkipCrlf(buffer, ref offset, ref count);
      int num = IrcString.IndexOf(buffer, (Func<byte, bool>) (@byte =>
      {
        if ((int) @byte != 13)
          return (int) @byte == 10;
        else
          return true;
      }), offset, count);
      if (num == -1)
      {
        if (count >= 512)
          parseResult = IrcStatementParseResult.StatementTooLong;
        return false;
      }
      else
      {
        IrcString ircString1 = new IrcString(buffer, offset, num - offset);
        offset += ircString1.Length + 1;
        count -= ircString1.Length + 1;
        IrcStatement.SkipCrlf(buffer, ref offset, ref count);
        if (num >= 512)
        {
          parseResult = IrcStatementParseResult.StatementTooLong;
          return false;
        }
        else
        {
          statement = new IrcStatement();
          if (ircString1.Length >= 1 && (int) ircString1[0] == 58)
          {
            IrcString[] ircStringArray = ircString1.Split((byte) 32, 2);
            IrcIdentity identity;
            if (IrcIdentity.TryParse(ircStringArray[0].Substring(1), out identity))
            {
              statement.Source = identity;
              ircString1 = ircStringArray.Length >= 2 ? ircStringArray[1] : IrcString.Empty;
            }
            else
              goto label_17;
          }
          IrcString[] ircStringArray1 = ircString1.Split((byte) 32, 2);
          statement.Command = ircStringArray1[0];
          IrcString[] ircStringArray2;
          for (IrcString ircString2 = ircStringArray1.Length >= 2 ? ircStringArray1[1] : IrcString.Empty; ircString2.Length > 0; ircString2 = ircStringArray2[1])
          {
            if ((int) ircString2[0] == 58)
            {
              statement.Parameters.Add(ircString2.Substring(1));
              break;
            }
            else
            {
              ircStringArray2 = ircString2.Split((byte) 32, 2);
              statement.Parameters.Add(ircStringArray2[0]);
              if (ircStringArray2.Length == 1)
                break;
            }
          }
          if (IrcValidation.ValidateStatement(statement, out errorMessage))
          {
            parseResult = IrcStatementParseResult.OK;
            return true;
          }
label_17:
          statement = (IrcStatement) null;
          parseResult = IrcStatementParseResult.InvalidStatement;
          return false;
        }
      }
    }

        /// <summary>
        /// Converts the IRC statement into a byte array, including the ending CR+LF.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// A byte array.
        /// </returns>
        public byte[] ToByteArray()
        {
            return (byte[])this.ToIrcString();
        }

        /// <summary>
        /// Converts the IRC statement into a byte array, including the ending CR+LF,
        ///             and additionally returns whether the string was truncated.
        /// 
        /// </summary>
        /// <param name="truncated"><c>true</c> if the string was too long and had to be truncated.</param>
        /// <returns>
        /// A byte array.
        /// </returns>
        public byte[] ToByteArray(out bool truncated)
        {
            return (byte[])this.ToIrcString(out truncated);
        }

        /// <summary>
        /// Converts the IRC statement into an IRC string containing all of its bytes,
        ///             including the ending CR+LF.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// An IRC string.
        /// </returns>
        public IrcString ToIrcString()
        {
            bool truncated;
            return this.ToIrcString(out truncated);
        }

        /// <summary>
        /// Converts the IRC statement into an IRC string containing all of its bytes,
        ///             including the ending CR+LF, and additionally returns whether the string was truncated.
        /// 
        /// </summary>
        /// <param name="truncated"><c>true</c> if the string was too long and had to be truncated.</param>
        /// <returns>
        /// An IRC string.
        /// </returns>
        public IrcString ToIrcString(out bool truncated)
        {
            string errorMessage;
            if (!IrcValidation.ValidateStatement(this, out errorMessage))
                throw new InvalidOperationException(errorMessage);
            truncated = false;
            IrcString ircString1 = IrcString.Empty;
            if (this.Source != (IrcIdentity)null)
                ircString1 += (IrcString)":" + this.Source.ToIrcString() + (IrcString)" ";
            IrcString ircString2 = ircString1 + this.Command;
            for (int index = 0; index < this.Parameters.Count; ++index)
            {
                IrcString ircString3 = this.Parameters[index];
                IrcString ircString4 = ircString2 + (IrcString)" ";
                if (index == this.Parameters.Count - 1 && ircString3.Contains((byte)32))
                    ircString4 += (IrcString)":";
                ircString2 = ircString4 + ircString3;
            }
            if (ircString2.Length > 510)
            {
                ircString2 = ircString2.Substring(0, 510);
                truncated = true;
            }
            return ircString2 + (IrcString)"\r\n";
        }
    }
}
