// Type: NetIrc2.Parsing.IrcStatementReceiver
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

using BotLeecher.NetIrc.Details;
using System;
using System.IO;

namespace BotLeecher.NetIrc.Parsing
{
    /// <summary>
    /// Receives IRC statements from a stream.
    /// 
    /// </summary>
    public sealed class IrcStatementReceiver
    {
        private byte[] _buffer = new byte[512];
        private int _count;
        private Stream _stream;

        /// <summary>
        /// Creates a new receiver.
        /// 
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        public IrcStatementReceiver(Stream stream)
        {
            ThrowExtensions.Null<Stream>(Throw.If, stream, "stream");
            this._stream = stream;
        }

        /// <summary>
        /// Tries to receive an IRC statement.
        /// 
        ///             A blocking read is used.
        ///             If you have a timeout set, <paramref name="parseResult"/> may be <see cref="F:NetIrc2.Parsing.IrcStatementParseResult.TimedOut"/>.
        /// 
        /// </summary>
        /// <param name="statement">The statement.</param><param name="parseResult">The parse result.</param>
        /// <returns>
        /// <c>true</c> if a complete IRC statement was received.
        /// </returns>
        public bool TryReceive(out IrcStatement statement, out IrcStatementParseResult parseResult)
        {
            if (this.TryReceiveFromBuffer(out statement, out parseResult))
                return true;
            IrcStatementParseResult statementParseResult = this.TryReceiveFromStream();
            if (statementParseResult == IrcStatementParseResult.OK)
                return this.TryReceiveFromBuffer(out statement, out parseResult);
            statement = (IrcStatement)null;
            parseResult = statementParseResult;
            return false;
        }

        private bool TryReceiveFromBuffer(out IrcStatement statement, out IrcStatementParseResult parseResult)
        {
            int offset = 0;
            bool flag = IrcStatement.TryParse(this._buffer, ref offset, this._count, out statement, out parseResult);
            if (offset != 0)
            {
                Array.Copy((Array)this._buffer, offset, (Array)this._buffer, 0, this._count - offset);
                this._count -= offset;
            }
            string val = System.Text.Encoding.UTF8.GetString(this._buffer);
            return flag;
        }

        private IrcStatementParseResult TryReceiveFromStream()
        {
            int num;
            try
            {
                num = this._stream.Read(this._buffer, this._count, this._buffer.Length - this._count);
            }
            catch (IOException ex)
            {
                num = 0;
            }
            catch (ObjectDisposedException ex)
            {
                num = 0;
            }
            catch (TimeoutException ex)
            {
                return IrcStatementParseResult.TimedOut;
            }
            if (num == 0)
                return IrcStatementParseResult.Disconnected;
            this._count += num;
            return IrcStatementParseResult.OK;
        }
    }
}
