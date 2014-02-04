// Type: NetIrc2.IdentServer
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

using BotLeecher.NetIrc.Details;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace BotLeecher.NetIrc
{
    /// <summary>
    /// Answers requests using the Ident protocol (RFC 1413).
    ///             Many IRC servers try to connect to a client's Ident server.
    /// 
    /// </summary>
    [Description("Responds to Ident requests as expected by some Internet Relay Chat servers.")]
    [Category("Network")]
    public class IdentServer : IComponent, IDisposable
    {
        private HashSet<IdentServer.Connection> _connections = new HashSet<IdentServer.Connection>();
        private IrcString _os;
        private IrcString _user;
        private TcpListener _listener;
        private IAsyncResult _listenResult;

        /// <summary>
        /// The name of the operating system running on the computer.
        /// 
        ///             By default, WIN32 will be used on Windows, and UNIX will be used elsewhere.
        /// 
        /// </summary>
        [AmbientValue(null)]
        public IrcString OperatingSystem
        {
            get
            {
                return this._os ?? (IrcString)(Environment.OSVersion.Platform == PlatformID.Win32NT ? "WIN32" : "UNIX");
            }
            set
            {
                this._os = value;
            }
        }

        /// <summary>
        /// The Ident user ID to reply with.
        /// 
        ///             Set this to match the IRC username.
        /// 
        /// </summary>
        [AmbientValue(null)]
        public IrcString UserID
        {
            get
            {
                return this._user ?? (IrcString)"netirc";
            }
            set
            {
                this._user = value;
            }
        }

        ISite IComponent.Site { get; set; }

        private event EventHandler Disposed;

        event EventHandler IComponent.Disposed
        {
            add
            {
                this.Disposed += value;
            }
            remove
            {
                this.Disposed -= value;
            }
        }

        /// <summary>
        /// Starts the Ident server.
        /// 
        /// </summary>
        /// <param name="port">The port to listen on. The standard port is 113.</param>
        public void Start(int port = 113)
        {
            ThrowExtensions.Negative(Throw.If, port, "port");
            this.Start(new IPEndPoint(IPAddress.Any, port));
        }

        /// <summary>
        /// Starts the Ident server, listening on the specified endpoint.
        /// 
        /// </summary>
        /// <param name="endPoint">The endpoint to listen on.</param>
        public void Start(IPEndPoint endPoint)
        {
            ThrowExtensions.Null<IPEndPoint>(Throw.If, endPoint, "endPoint");
            this._listener = new TcpListener(endPoint);
            this._listener.Start();
            this.Accept();
        }

        private void Accept()
        {
            try
            {
                this._listenResult = this._listener.BeginAcceptTcpClient(new AsyncCallback(this.HandleAcceptTcpClient), (object)null);
            }
            catch (ObjectDisposedException ex)
            {
            }
        }

        /// <summary>
        /// Stops the Ident server and disconnects all connected clients.
        /// 
        /// </summary>
        public void Stop()
        {
            if (this._listener != null)
                this._listener.Stop();
            if (this._listenResult != null)
                this._listenResult.AsyncWaitHandle.WaitOne();
            lock (this._connections)
            {
                foreach (IdentServer.Connection item_0 in Enumerable.ToArray<IdentServer.Connection>((IEnumerable<IdentServer.Connection>)this._connections))
                    item_0.Stop();
            }
        }

        private void HandleAcceptTcpClient(IAsyncResult result)
        {
            TcpClient tcpClient;
            try
            {
                tcpClient = this._listener.EndAcceptTcpClient(result);
            }
            catch (ObjectDisposedException ex)
            {
                return;
            }
            NetworkStream stream = tcpClient.GetStream();
            new IdentServer.Connection()
            {
                Server = this,
                Stream = stream
            }.Start();
            this.Accept();
        }

        private bool ShouldSerializeOperatingSystem()
        {
            return this._os != (IrcString)null;
        }

        private bool ShouldSerializeUserID()
        {
            return this._user != (IrcString)null;
        }

        void IDisposable.Dispose()
        {
            this.Stop();
            EventHandler eventHandler = this.Disposed;
            if (eventHandler == null)
                return;
            eventHandler((object)this, EventArgs.Empty);
        }

        protected virtual IrcString GetQueryResponse(IrcString query)
        {
            IrcString[] ircStringArray = query.Split((byte)44);
            if (ircStringArray.Length != 2)
                return query + (IrcString)" : ERROR : UNKNOWN-ERROR";
            int result1;
            int result2;
            if (!int.TryParse((string)ircStringArray[0], out result1) || result1 <= 0 || (!int.TryParse((string)ircStringArray[1], out result2) || result2 <= 0))
                return query + (IrcString)" : ERROR : INVALID-PORT";
            IrcString ircString1 = this.OperatingSystem.Split((byte)58, 2)[0];
            IrcString ircString2 = this.UserID.Split((byte)58, 2)[0];
            return query + (IrcString)" : USERID : " + ircString1 + (IrcString)" : " + ircString2;
        }

        private sealed class Connection
        {
            private byte[] _inBuffer = new byte[128];
            private int _inOffset;
            private IAsyncResult _readResult;
            private IAsyncResult _writeResult;

            public IdentServer Server { get; set; }

            public NetworkStream Stream { get; set; }

            public void Start()
            {
                lock (this.Server._connections)
                    this.Server._connections.Add(this);
                try
                {
                    this._readResult = this.Stream.BeginRead(this._inBuffer, this._inOffset, this._inBuffer.Length - this._inOffset, new AsyncCallback(this.HandleRead), (object)null);
                }
                catch (IOException ex)
                {
                    this.Stop();
                }
                catch (ObjectDisposedException ex)
                {
                    this.Stop();
                }
            }

            public void Stop()
            {
                ((Stream)this.Stream).Close();
                IAsyncResult asyncResult1 = this._readResult;
                if (asyncResult1 != null)
                    asyncResult1.AsyncWaitHandle.WaitOne();
                IAsyncResult asyncResult2 = this._writeResult;
                if (asyncResult2 != null)
                    asyncResult2.AsyncWaitHandle.WaitOne();
                lock (this.Server._connections)
                    this.Server._connections.Remove(this);
            }

            private void HandleRead(IAsyncResult result)
            {
                int num = 0;
                try
                {
                    num = this.Stream.EndRead(result);
                }
                catch (IOException ex)
                {
                }
                catch (ObjectDisposedException ex)
                {
                }
                if (num == 0)
                {
                    this.Stop();
                }
                else
                {
                    this._inOffset += num;
                    IrcString queryResponse;
                    do
                    {
                        IrcString query;
                        do
                        {
                            int length = IrcString.IndexOf(this._inBuffer, (Func<byte, bool>)(@byte =>
                            {
                                if ((int)@byte != 13)
                                    return (int)@byte == 10;
                                else
                                    return true;
                            }), 0, this._inOffset);
                            if (length == -1)
                            {
                                if (this._inOffset == this._inBuffer.Length)
                                {
                                    this.Stop();
                                    return;
                                }
                                else
                                {
                                    this.Start();
                                    return;
                                }
                            }
                            else
                            {
                                query = new IrcString(this._inBuffer, 0, length);
                                Array.Copy((Array)this._inBuffer, length + 1, (Array)this._inBuffer, 0, this._inOffset - (length + 1));
                                this._inOffset -= length + 1;
                            }
                        }
                        while (query.Length <= 0 || query.Contains((byte)0));
                        queryResponse = this.Server.GetQueryResponse(query);
                    }
                    while (!(queryResponse != (IrcString)null) || queryResponse.Length <= 0);
                    byte[] buffer = (queryResponse + (IrcString)"\r\n").ToByteArray();
                    try
                    {
                        this._writeResult = this.Stream.BeginWrite(buffer, 0, buffer.Length, new AsyncCallback(this.HandleWrite), (object)null);
                    }
                    catch (IOException ex)
                    {
                        this.Stop();
                    }
                    catch (ObjectDisposedException ex)
                    {
                        this.Stop();
                    }
                }
            }

            private void HandleWrite(IAsyncResult result)
            {
                try
                {
                    this.Stream.EndWrite(result);
                }
                catch (IOException ex)
                {
                    this.Stop();
                    return;
                }
                catch (ObjectDisposedException ex)
                {
                    this.Stop();
                    return;
                }
                this.Start();
            }
        }
    }
}
