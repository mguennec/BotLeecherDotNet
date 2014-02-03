// Type: NetIrc2.IrcClient
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

using BotLeecher.NetIrc.Details;
using BotLeecher.NetIrc.Events;
using BotLeecher.NetIrc.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;

namespace BotLeecher.NetIrc
{
    /// <summary>
    /// Communicates with an Internet Relay Chat server.
    /// 
    /// </summary>
    [Category("Network")]
    [Description("Communicates with an Internet Relay Chat server.")]
    public class IrcClient : IComponent, IDisposable
    {
        private const int _ctcpMask = 65535;
        private IrcClient.Context _context;
        private IrcString _clientVer;
        private ushort _ctcpPingOffset;
        private static double _ctcpTimeOffset;
        public static readonly int[] Ports = { 2700, 2701, 2702, 2703, 2704, 2705 };

        ISite IComponent.Site { get; set; }

        /// <summary>
        /// The client version. This will be sent in reply to a CTCP VERSION query.
        /// 
        /// </summary>
        [AmbientValue(null)]
        public IrcString ClientVersion
        {
            get
            {
                return this._clientVer ?? (IrcString)("NetIRC2 " + ((object)Assembly.GetExecutingAssembly().GetName().Version).ToString());
            }
            set
            {
                this._clientVer = value;
            }
        }

        /// <summary>
        /// Whether the client is connected to a server.
        /// 
        /// </summary>
        [Browsable(false)]
        public bool IsConnected { get; private set; }

        /// <summary>
        /// The synchronization object for sending IRC commands.
        /// 
        /// </summary>
        [Browsable(false)]
        public object SyncRoot { get; private set; }

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
        /// Called when a connection is established.
        /// 
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// Called when the connection is terminated.
        /// 
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Called when the server has begun sending the channel list.
        /// 
        /// </summary>
        public event EventHandler GotChannelListBegin;

        /// <summary>
        /// Called for each entry of the channel list.
        /// 
        /// </summary>
        public event EventHandler<ChannelListEntryEventArgs> GotChannelListEntry;

        /// <summary>
        /// Called when the server has finished sending the channel list.
        /// 
        /// </summary>
        public event EventHandler GotChannelListEnd;

        /// <summary>
        /// Called when a channel's topic changes.
        /// 
        /// </summary>
        public event EventHandler<ChannelTopicChangeEventArgs> GotChannelTopicChange;

        /// <summary>
        /// Called when someone sends a chat action message.
        /// 
        /// </summary>
        public event EventHandler<ChatMessageEventArgs> GotChatAction;

        /// <summary>
        /// Called when the client receives an invitation to join a channel.
        /// 
        /// </summary>
        public event EventHandler<InvitationEventArgs> GotInvitation;

        /// <summary>
        /// Called when an error occurs.
        /// 
        /// </summary>
        public event EventHandler<IrcErrorEventArgs> GotIrcError;

        /// <summary>
        /// Called when someone joins a channel.
        /// 
        /// </summary>
        public event EventHandler<JoinLeaveEventArgs> GotJoinChannel;

        /// <summary>
        /// Called when someone leaves a channel.
        /// 
        /// </summary>
        public event EventHandler<JoinLeaveEventArgs> GotLeaveChannel;

        /// <summary>
        /// Called when someone sends a message.
        /// 
        /// </summary>
        public event EventHandler<ChatMessageEventArgs> GotMessage;

        /// <summary>
        /// Called when a channel or user's mode is changed.
        /// 
        /// </summary>
        public event EventHandler<ModeEventArgs> GotMode;

        /// <summary>
        /// Called when the server has begun sending the Message of the Day.
        /// 
        /// </summary>
        public event EventHandler GotMotdBegin;

        /// <summary>
        /// Called for each line of the Message of the Day sent by the server.
        /// 
        /// </summary>
        public event EventHandler<SimpleMessageEventArgs> GotMotdText;

        /// <summary>
        /// Called when the server has finished sending the Message of the Day.
        /// 
        /// </summary>
        public event EventHandler GotMotdEnd;

        /// <summary>
        /// Called when someone changes their name.
        /// 
        /// </summary>
        public event EventHandler<NameChangeEventArgs> GotNameChange;

        /// <summary>
        /// Called when the server is sending a channel's user list.
        /// 
        /// </summary>
        public event EventHandler<NameListReplyEventArgs> GotNameListReply;

        /// <summary>
        /// Called at the completion of a channel's user list.
        /// 
        /// </summary>
        public event EventHandler<NameListEndEventArgs> GotNameListEnd;

        /// <summary>
        /// Called when someone sends a notice. Notices differ from
        ///             ordinary messages in that, by convention, one should not
        ///             send an automated reply in response (such as 'I am away
        ///             from the keyboard.').
        /// 
        /// </summary>
        public event EventHandler<ChatMessageEventArgs> GotNotice;

        /// <summary>
        /// Called on a user's reply to a ping.
        /// 
        /// </summary>
        public event EventHandler<PingReplyEventArgs> GotPingReply;

        /// <summary>
        /// Called when a user is kicked from a channel.
        /// 
        /// </summary>
        public event EventHandler<KickEventArgs> GotUserKicked;

        /// <summary>
        /// Called when a user disconnects from the server.
        /// 
        /// </summary>
        public event EventHandler<QuitEventArgs> GotUserQuit;

        /// <summary>
        /// Called when the server sends the welcome message.
        /// 
        /// </summary>
        public event EventHandler<SimpleMessageEventArgs> GotWelcomeMessage;

        static IrcClient()
        {
            IrcClient.CtcpTimeInit();
        }

        /// <summary>
        /// Creates a new IRC client.
        /// 
        /// </summary>
        public IrcClient()
        {
            this.CtcpPingInit();
            this.SyncRoot = new object();
        }

        void IDisposable.Dispose()
        {
            this.Close();
            EventHandler eventHandler = this.Disposed;
            if (eventHandler == null)
                return;
            eventHandler((object)this, EventArgs.Empty);
        }

        /// <summary>
        /// Connects to an IRC server.
        /// 
        /// </summary>
        /// <param name="hostname">The server hostname.</param><param name="port">The server port.</param><param name="options">Options for the connection, if any, or <c>null</c>.</param>
        public void Connect(string hostname, int port = 6667, IrcClientConnectionOptions options = null)
        {
            ThrowExtensions.Negative(ThrowExtensions.Null<string>(Throw.If, hostname, "hostname"), port, "port");
            this.Connect((Action<TcpClient>)(client => client.Connect(hostname, port)), options);
        }

        /// <summary>
        /// Connects to an IRC server specified by an endpoint.
        /// 
        /// </summary>
        /// <param name="endPoint">The IP endpoint to connect to.</param><param name="options">Options for the connection, if any, or <c>null</c>.</param>
        public void Connect(IPEndPoint endPoint, IrcClientConnectionOptions options = null)
        {
            ThrowExtensions.Null<IPEndPoint>(Throw.If, endPoint, "endPoint");
            this.Connect((Action<TcpClient>)(client => client.Connect(endPoint)), options);
        }

        private void Connect(Action<TcpClient> connectTcpClientCallback, IrcClientConnectionOptions options)
        {
            TcpClient tcpClient = new TcpClient();
            connectTcpClientCallback(tcpClient);
            try
            {
                this.Connect((Stream)tcpClient.GetStream(), options);
            }
            catch (Exception ex)
            {
                tcpClient.Close();
                throw;
            }
        }

        /// <summary>
        /// Connects to an IRC server specified by a stream.
        /// 
        /// </summary>
        /// <param name="stream">The stream.</param><param name="options">Options for the connection, if any, or <c>null</c>.</param>
        public void Connect(Stream stream, IrcClientConnectionOptions options = null)
        {
            ThrowExtensions.Null<Stream>(Throw.If, stream, "stream");
            this.Close();
            if (options == null)
                options = new IrcClientConnectionOptions();
            if (options.Ssl)
            {
                if (options.SslHostname == null)
                    throw new ArgumentException("If Ssl is true, SslHostname must be set.", "options");
                SslStream sslStream = new SslStream(stream, false, options.SslCertificateValidationCallback);
                sslStream.AuthenticateAsClient(options.SslHostname);
                stream = (Stream)sslStream;
            }
            IrcClient.Context context = new IrcClient.Context()
            {
                ReceiverThread = new Thread(new ParameterizedThreadStart(this.ThreadReceiver)),
                StartEvent = new ManualResetEvent(false),
                Stream = stream,
                SynchronizationContext = options.SynchronizationContext
            };
            this._context = context;
            context.ReceiverThread.IsBackground = true;
            context.ReceiverThread.Start((object)context);
            try
            {
                this.IsConnected = true;
                this.OnConnected();
            }
            finally
            {
                context.StartEvent.Set();
            }
        }

        /// <summary>
        /// Sends a CTCP command to the specified user or channel.
        /// 
        /// </summary>
        /// <param name="recipient">The user or channel to send the command to.</param><param name="command">The CTCP command.</param><param name="parameters">The CTCP command parameters.</param><param name="escapeParameters"><c>true</c> to quote parameters with spaces in them, and escape backslashes and quotation marks.
        ///             </param>
        public void CtcpCommand(IrcString recipient, IrcString command, IrcString[] parameters, bool escapeParameters = true)
        {
            ThrowExtensions.NullElements<IrcString>(ThrowExtensions.Null<IrcString>(ThrowExtensions.Null<IrcString>(Throw.If, recipient, "recipient"), command, "command"), (IEnumerable<IrcString>)parameters, "parameters");
            this.Message(recipient, IrcClient.CtcpEncode(command, parameters, escapeParameters));
        }

        /// <summary>
        /// Replies to a CTCP command from a user or channel.
        /// 
        /// </summary>
        /// <param name="recipient">The user or channel to send the reply to.</param><param name="command">The CTCP command.</param><param name="parameters">The CTCP command reply parameters.</param><param name="escapeParameters"><c>true</c> to quote parameters with spaces in them, and escape backslashes and quotation marks.
        ///             </param>
        public void CtcpReply(IrcString recipient, IrcString command, IrcString[] parameters, bool escapeParameters = true)
        {
            ThrowExtensions.NullElements<IrcString>(ThrowExtensions.Null<IrcString>(ThrowExtensions.Null<IrcString>(Throw.If, recipient, "recipient"), command, "command"), (IEnumerable<IrcString>)parameters, "parameters");
            this.Notice(recipient, (string)IrcClient.CtcpEncode(command, parameters, escapeParameters));
        }

        /// <summary>
        /// Sends a DCC command to the specified user or channel.
        /// 
        /// </summary>
        /// <param name="recipient">The user or channel to send the command to.</param><param name="command">The DCC command.</param><param name="parameters">The DCC command parameters.</param>
        public void DccCommand(IrcString recipient, IrcString command, params IrcString[] parameters)
        {
            ThrowExtensions.NullElements<IrcString>(ThrowExtensions.Null<IrcString>(ThrowExtensions.Null<IrcString>(Throw.If, recipient, "recipient"), command, "command"), (IEnumerable<IrcString>)parameters, "parameters");
            this.CtcpCommand(recipient, (IrcString)"DCC", Enumerable.ToArray<IrcString>(Enumerable.Concat<IrcString>((IEnumerable<IrcString>)new IrcString[1]
      {
        command
      }, (IEnumerable<IrcString>)parameters)), 1 != 0);
        }

        /// <summary>
        /// Constructs and sends an IRC command to the server.
        /// 
        /// </summary>
        /// <param name="command">The command to send.</param><param name="parameters">The command's parameters.</param>
        /// <returns>
        /// <c>true</c> if the command was sent successfully.
        /// </returns>
        public bool IrcCommand(IrcString command, params IrcString[] parameters)
        {
            ThrowExtensions.Null<IrcString[]>(ThrowExtensions.Null<IrcString>(Throw.If, command, "command"), parameters, "parameters");
            return this.IrcCommand(new IrcStatement((IrcIdentity)null, command, parameters));
        }

        /// <summary>
        /// Sends a premade IRC statement to the server.
        /// 
        /// </summary>
        /// <param name="statement">The statement to send.</param>
        /// <returns>
        /// <c>true</c> if the statement was sent successfully.
        /// </returns>
        public bool IrcCommand(IrcStatement statement)
        {
            ThrowExtensions.Null<IrcStatement>(Throw.If, statement, "statement");
            byte[] buffer = statement.ToByteArray();
            lock (this.SyncRoot)
            {
                IrcClient.Context local_1 = this._context;
                if (local_1 == null)
                    return false;
                try
                {
                    local_1.Stream.Write(buffer, 0, buffer.Length);
                }
                catch (IOException exception_0)
                {
                    return false;
                }
                catch (ObjectDisposedException exception_1)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Closes the network connection.
        /// 
        /// </summary>
        public void Close()
        {
            IrcClient.Context context = this._context;
            if (context == null)
                return;
            context.Stream.Close();
            context.ReceiverThread.Join();
            this._context = (IrcClient.Context)null;
        }

        private void ThreadReceiver(object parameter)
        {
            IrcClient.Context context = (IrcClient.Context)parameter;
            NetIrc.Parsing.IrcStatementReceiver statementReceiver = new NetIrc.Parsing.IrcStatementReceiver(context.Stream);
            context.StartEvent.Set();
            while (true)
            {
                IrcStatement statement;
                IrcStatementParseResult parseResult;
                while (!statementReceiver.TryReceive(out statement, out parseResult))
                {
                    if (parseResult != IrcStatementParseResult.NothingToParse)
                    {
                        this.IsConnected = false;
                        this.OnClosed();
                        return;
                    }
                }
                this.OnIrcStatementReceived(statement);
            }
        }

        private bool ShouldSerializeClientVersion()
        {
            return this._clientVer != (IrcString)null;
        }

        private void CtcpPingInit()
        {
            byte[] data = new byte[2];
            new RNGCryptoServiceProvider().GetBytes(data);
            this._ctcpPingOffset = BitConverter.ToUInt16(data, 0);
        }

        private int CtcpPingGetTimestamp()
        {
            return Environment.TickCount + (int)this._ctcpPingOffset & (int)ushort.MaxValue;
        }

        private int CtcpPingGetTimeDifference(int start, int current)
        {
            return current - start & (int)ushort.MaxValue;
        }

        protected virtual void OnConnected()
        {
            this.RaiseConnected();
        }

        protected void RaiseConnected()
        {
            this.Dispatch(this.Connected);
        }

        protected virtual void OnClosed()
        {
            this.RaiseClosed();
        }

        protected void RaiseClosed()
        {
            this.Dispatch(this.Closed);
        }

        protected virtual void OnGotChannelListBegin()
        {
            this.RaiseGotChannelListBegin();
        }

        protected void RaiseGotChannelListBegin()
        {
            this.Dispatch(this.GotChannelListBegin);
        }

        protected virtual void OnGotChannelListEntry(ChannelListEntryEventArgs e)
        {
            this.RaiseGotChannelListEntry(e);
        }

        protected void RaiseGotChannelListEntry(ChannelListEntryEventArgs e)
        {
            this.Dispatch<ChannelListEntryEventArgs>(this.GotChannelListEntry, e);
        }

        protected virtual void OnGotChannelListEnd()
        {
            this.RaiseGotChannelListEnd();
        }

        protected void RaiseGotChannelListEnd()
        {
            this.Dispatch(this.GotChannelListEnd);
        }

        protected virtual void OnGotChannelTopicChange(ChannelTopicChangeEventArgs e)
        {
            this.RaiseGotChannelTopicChange(e);
        }

        protected void RaiseGotChannelTopicChange(ChannelTopicChangeEventArgs e)
        {
            this.Dispatch<ChannelTopicChangeEventArgs>(this.GotChannelTopicChange, e);
        }

        protected virtual void OnGotChatAction(ChatMessageEventArgs e)
        {
            this.RaiseGotChatAction(e);
        }

        protected void RaiseGotChatAction(ChatMessageEventArgs e)
        {
            this.Dispatch<ChatMessageEventArgs>(this.GotChatAction, e);
        }

        protected virtual void OnGotInvitation(InvitationEventArgs e)
        {
            this.RaiseGotInvitation(e);
        }

        protected void RaiseGotInvitation(InvitationEventArgs e)
        {
            this.Dispatch<InvitationEventArgs>(this.GotInvitation, e);
        }

        protected virtual void OnGotIrcError(IrcErrorEventArgs e)
        {
            this.RaiseGotIrcError(e);
        }

        protected void RaiseGotIrcError(IrcErrorEventArgs e)
        {
            this.Dispatch<IrcErrorEventArgs>(this.GotIrcError, e);
        }

        protected virtual void OnGotJoinChannel(JoinLeaveEventArgs e)
        {
            this.RaiseGotJoinChannel(e);
        }

        protected void RaiseGotJoinChannel(JoinLeaveEventArgs e)
        {
            this.Dispatch<JoinLeaveEventArgs>(this.GotJoinChannel, e);
        }

        protected virtual void OnGotLeaveChannel(JoinLeaveEventArgs e)
        {
            this.RaiseGotLeaveChannel(e);
        }

        protected void RaiseGotLeaveChannel(JoinLeaveEventArgs e)
        {
            this.Dispatch<JoinLeaveEventArgs>(this.GotLeaveChannel, e);
        }

        protected virtual void OnGotMessage(ChatMessageEventArgs e)
        {
            this.RaiseGotMessage(e);
        }

        protected void RaiseGotMessage(ChatMessageEventArgs e)
        {
            this.Dispatch<ChatMessageEventArgs>(this.GotMessage, e);
        }

        protected virtual void OnGotMode(ModeEventArgs e)
        {
            this.RaiseGotMode(e);
        }

        protected void RaiseGotMode(ModeEventArgs e)
        {
            this.Dispatch<ModeEventArgs>(this.GotMode, e);
        }

        protected virtual void OnGotMotdBegin()
        {
            this.RaiseGotMotdBegin();
        }

        protected void RaiseGotMotdBegin()
        {
            this.Dispatch(this.GotMotdBegin);
        }

        protected virtual void OnGotMotdText(SimpleMessageEventArgs e)
        {
            this.RaiseGotMotdText(e);
        }

        protected void RaiseGotMotdText(SimpleMessageEventArgs e)
        {
            this.Dispatch<SimpleMessageEventArgs>(this.GotMotdText, e);
        }

        protected virtual void OnGotMotdEnd()
        {
            this.RaiseGotMotdEnd();
        }

        protected void RaiseGotMotdEnd()
        {
            this.Dispatch(this.GotMotdEnd);
        }

        protected virtual void OnGotNameChange(NameChangeEventArgs e)
        {
            this.RaiseGotNameChange(e);
        }

        protected void RaiseGotNameChange(NameChangeEventArgs e)
        {
            this.Dispatch<NameChangeEventArgs>(this.GotNameChange, e);
        }

        protected virtual void OnGotNameListReply(NameListReplyEventArgs e)
        {
            this.RaiseGotNameListReply(e);
        }

        protected void RaiseGotNameListReply(NameListReplyEventArgs e)
        {
            this.Dispatch<NameListReplyEventArgs>(this.GotNameListReply, e);
        }

        protected virtual void OnGotNameListEnd(NameListEndEventArgs e)
        {
            this.RaiseGotNameListEnd(e);
        }

        protected void RaiseGotNameListEnd(NameListEndEventArgs e)
        {
            this.Dispatch<NameListEndEventArgs>(this.GotNameListEnd, e);
        }

        protected virtual void OnGotNotice(ChatMessageEventArgs e)
        {
            this.RaiseGotNotice(e);
        }

        protected void RaiseGotNotice(ChatMessageEventArgs e)
        {
            this.Dispatch<ChatMessageEventArgs>(this.GotNotice, e);
        }

        protected virtual void OnGotPingReply(PingReplyEventArgs e)
        {
            this.RaiseGotPingReply(e);
        }

        protected void RaiseGotPingReply(PingReplyEventArgs e)
        {
            this.Dispatch<PingReplyEventArgs>(this.GotPingReply, e);
        }

        protected virtual void OnGotUserKicked(KickEventArgs e)
        {
            this.RaiseGotUserKicked(e);
        }

        protected virtual void OnGotUserQuit(QuitEventArgs e)
        {
            this.RaiseGotUserQuit(e);
        }

        protected void RaiseGotUserQuit(QuitEventArgs e)
        {
            this.Dispatch<QuitEventArgs>(this.GotUserQuit, e);
        }

        protected void RaiseGotUserKicked(KickEventArgs e)
        {
            this.Dispatch<KickEventArgs>(this.GotUserKicked, e);
        }

        protected virtual void OnGotWelcomeMessage(SimpleMessageEventArgs e)
        {
            this.RaiseGotWelcomeMessage(e);
        }

        protected void RaiseGotWelcomeMessage(SimpleMessageEventArgs e)
        {
            this.Dispatch<SimpleMessageEventArgs>(this.GotWelcomeMessage, e);
        }

        private void Dispatch(EventHandler @event)
        {
            if (@event == null)
                return;
            SynchronizationContext synchronizationContext = this._context.SynchronizationContext;
            if (synchronizationContext == null)
                @event((object)this, EventArgs.Empty);
            else
                synchronizationContext.Post((SendOrPostCallback)(_ => @event((object)this, EventArgs.Empty)), (object)null);
        }

        private void Dispatch<T>(EventHandler<T> @event, T e) where T : EventArgs
        {
            if (@event == null)
                return;
            SynchronizationContext synchronizationContext = this._context.SynchronizationContext;
            if (synchronizationContext == null)
                @event((object)this, e);
            else
                synchronizationContext.Post((SendOrPostCallback)(_ => @event((object)this, e)), (object)null);
        }

        private static void CtcpTimeInit()
        {
            byte[] data = new byte[4];
            new RNGCryptoServiceProvider().GetBytes(data);
            IrcClient._ctcpTimeOffset = ((double)BitConverter.ToUInt32(data, 0) / (double)uint.MaxValue - 0.5) * 600.0;
        }

        private DateTime CtcpTimeGetNow()
        {
            return DateTime.Now.AddSeconds(IrcClient._ctcpTimeOffset);
        }

        /// <summary>
        /// Logs in to the server.
        /// 
        /// </summary>
        /// <param name="username">A username. If you aren't using a password, this can be anything you want.</param><param name="realname">Your real name, or some made up name.</param><param name="nickname">The IRC nickname to use.</param><param name="hostname">The hostname to send, or <c>null</c> to send a default value.</param><param name="servername">The servername to send, or <c>null</c> to send a default value.</param><param name="password">The connection password, or <c>null</c> to not use one.</param>
        public void LogIn(IrcString username, IrcString realname, IrcString nickname, IrcString hostname = null, IrcString servername = null, IrcString password = null)
        {
            ThrowExtensions.Null<IrcString>(ThrowExtensions.Null<IrcString>(ThrowExtensions.Null<IrcString>(Throw.If, username, "username"), realname, "realname"), nickname, "nickname");
            lock (this.SyncRoot)
            {
                if (password != (IrcString)null)
                    this.IrcCommand((IrcString)"PASS", new IrcString[1]
          {
            password
          });
                this.IrcCommand((IrcString)"NICK", new IrcString[1]
        {
          nickname
        });
                this.IrcCommand((IrcString)"USER", username, hostname ?? (IrcString)"0", servername ?? (IrcString)"*", realname);
            }
        }

        /// <summary>
        /// Changes the channel topic.
        /// 
        /// </summary>
        /// <param name="channel">The channel whose topic to change.</param><param name="newTopic">The new channel topic.</param>
        public void ChangeChannelTopic(IrcString channel, IrcString newTopic)
        {
            ThrowExtensions.Null<IrcString>(ThrowExtensions.Null<IrcString>(Throw.If, channel, "channel"), newTopic, "newTopic");
            this.IrcCommand((IrcString)"TOPIC", channel, newTopic);
        }

        /// <summary>
        /// Changes the client's nickname.
        /// 
        /// </summary>
        /// <param name="newName">The nickname to change to.</param>
        public void ChangeName(IrcString newName)
        {
            ThrowExtensions.Null<IrcString>(Throw.If, newName, "newName");
            this.IrcCommand((IrcString)"NICK", new IrcString[1]
      {
        newName
      });
        }

        /// <summary>
        /// Sends an action message to the specified user or channel.
        /// 
        /// </summary>
        /// <param name="recipient">The user or channel to send the action message to.</param><param name="message">The message to send.</param>
        public void ChatAction(IrcString recipient, IrcString message)
        {
            ThrowExtensions.Null<IrcString>(ThrowExtensions.Null<IrcString>(Throw.If, recipient, "recipient"), message, "message");
            this.CtcpCommand(recipient, (IrcString)"ACTION", new IrcString[1]
      {
        message
      }, 0 != 0);
        }

        /// <summary>
        /// Invites the specified user to the channel. Channel operator access
        ///             may be required.
        /// 
        /// </summary>
        /// <param name="user">The user to invite.</param><param name="channel">The channel to invite the user to.</param>
        public void Invite(IrcString user, IrcString channel)
        {
            ThrowExtensions.Null<IrcString>(ThrowExtensions.Null<IrcString>(Throw.If, user, "user"), channel, "channel");
            this.IrcCommand((IrcString)"INVITE", user, channel);
        }

        /// <summary>
        /// Joins the specified channel.
        /// 
        /// </summary>
        /// <param name="channel">The channel to join.</param><param name="key">The channel key, or <c>null</c> if a key is unnecessary.</param>
        public void Join(IrcString channel, IrcString key = null)
        {
            ThrowExtensions.Null<IrcString>(Throw.If, channel, "channel");
            IrcClient ircClient = this;
            IrcString command = (IrcString)"JOIN";
            IrcString[] ircStringArray;
            if (!(key != (IrcString)null))
                ircStringArray = new IrcString[1]
        {
          channel
        };
            else
                ircStringArray = new IrcString[2]
        {
          channel,
          key
        };
            ircClient.IrcCommand(command, ircStringArray);
        }

        /// <summary>
        /// Kicks the specified user from the channel. Channel operator access may be required.
        /// 
        /// </summary>
        /// <param name="user">The user to kick.</param><param name="channel">The channel to kick the user from.</param><param name="reason">The reason the user was kicked, or <c>null</c> to give no reason.</param>
        public void Kick(IrcString user, IrcString channel, IrcString reason)
        {
            ThrowExtensions.Null<IrcString>(ThrowExtensions.Null<IrcString>(Throw.If, user, "user"), channel, "channel");
            IrcClient ircClient = this;
            IrcString command = (IrcString)"KICK";
            IrcString[] ircStringArray;
            if (!(reason != (IrcString)null))
                ircStringArray = new IrcString[2]
        {
          channel,
          user
        };
            else
                ircStringArray = new IrcString[3]
        {
          channel,
          user,
          reason
        };
            ircClient.IrcCommand(command, ircStringArray);
        }

        /// <summary>
        /// Leaves the specified channel.
        /// 
        /// </summary>
        /// <param name="channel">The channel to leave.</param>
        public void Leave(IrcString channel)
        {
            ThrowExtensions.Null<IrcString>(Throw.If, channel, "channel");
            this.IrcCommand((IrcString)"PART", new IrcString[1]
      {
        channel
      });
        }

        /// <summary>
        /// Requests a listing of available channels on the server.
        /// 
        /// </summary>
        public void ListChannels()
        {
            this.IrcCommand((IrcString)"LIST", new IrcString[0]);
        }

        /// <summary>
        /// Sends a message to the specified user or channel.
        /// 
        /// </summary>
        /// <param name="recipient">The user or channel to send the message to.</param><param name="message">The message to send.</param>
        public void Message(IrcString recipient, IrcString message)
        {
            ThrowExtensions.Null<IrcString>(ThrowExtensions.Null<IrcString>(Throw.If, recipient, "recipient"), message, "message");
            this.IrcCommand((IrcString)"PRIVMSG", recipient, message);
        }

        /// <summary>
        /// Changes a channel or user's mode.
        /// 
        /// </summary>
        /// <param name="target">The channel or user to change the mode of.</param><param name="command">The mode change, for example +o or +v.</param><param name="parameters">The mode change parameters.</param>
        public void Mode(IrcString target, IrcString command, params IrcString[] parameters)
        {
            ThrowExtensions.NullElements<IrcString>(ThrowExtensions.Null<IrcString>(ThrowExtensions.Null<IrcString>(Throw.If, target, "target"), command, "command"), (IEnumerable<IrcString>)parameters, "parameters");
            this.IrcCommand((IrcString)"MODE", Enumerable.ToArray<IrcString>(Enumerable.Concat<IrcString>((IEnumerable<IrcString>)new IrcString[2]
      {
        target,
        command
      }, (IEnumerable<IrcString>)parameters)));
        }

        /// <summary>
        /// Sends a notice to the specified user.
        /// 
        /// </summary>
        /// <param name="recipient">The user to send the notice to.</param><param name="message">The message to send.</param>
        public void Notice(IrcString recipient, string message)
        {
            ThrowExtensions.Null<string>(ThrowExtensions.Null<IrcString>(Throw.If, recipient, "recipient"), message, "message");
            this.IrcCommand((IrcString)"NOTICE", recipient, (IrcString)message);
        }

        /// <summary>
        /// Pings the specified user.
        /// 
        /// </summary>
        /// <param name="userToPing">The user to ping.</param>
        public void Ping(IrcString userToPing)
        {
            ThrowExtensions.Null<IrcString>(Throw.If, userToPing, "userToPing");
            this.CtcpCommand(userToPing, (IrcString)"PING", new IrcString[1]
      {
        (IrcString) this.CtcpPingGetTimestamp().ToString()
      }, 1 != 0);
        }

        /// <summary>
        /// Logs out from the server.
        /// 
        /// </summary>
        /// <param name="quitMessage">The quit message, or <c>null</c>.</param>
        public void LogOut(string quitMessage = null)
        {
            IrcClient ircClient = this;
            IrcString command = (IrcString)"QUIT";
            IrcString[] ircStringArray;
            if (quitMessage == null)
                ircStringArray = new IrcString[0];
            else
                ircStringArray = new IrcString[1]
        {
          (IrcString) quitMessage
        };
            ircClient.IrcCommand(command, ircStringArray);
        }

        private static IrcString CtcpEncode(IrcString command, IrcString[] parameters, bool escapeParameters)
        {
            return (IrcString)"\x0001" + command + IrcString.Join((IrcString)"", Enumerable.ToArray<IrcString>(Enumerable.Select<IrcString, IrcString>((IEnumerable<IrcString>)parameters, (Func<IrcString, IrcString>)(p => (IrcString)" " + (escapeParameters ? IrcClient.CtcpEscapeParameter(p) : p))))) + (IrcString)"\x0001";
        }

        private static IrcString CtcpEscapeParameter(IrcString parameter)
        {
            parameter = new IrcString(Enumerable.ToArray<byte>(Enumerable.SelectMany<byte, byte>((IEnumerable<byte>)parameter, (Func<byte, IEnumerable<byte>>)(@byte =>
            {
                if ((int)@byte == 0 || (int)@byte == 1 || ((int)@byte == 13 || (int)@byte == 10))
                    return (IEnumerable<byte>)new byte[0];
                if ((int)@byte == 92 || (int)@byte == 34)
                    return (IEnumerable<byte>)new byte[2]
          {
            (byte) 92,
            @byte
          };
                else
                    return (IEnumerable<byte>)new byte[1]
          {
            @byte
          };
            }))));
            if (!parameter.Contains((byte)32))
                return parameter;
            else
                return (IrcString)"\"" + parameter + (IrcString)"\"";
        }

        private static bool TryCtcpDecode(IrcString message, out IrcString command, out IrcString[] parameters, out IrcString rawParameter)
        {
            command = (IrcString)null;
            rawParameter = (IrcString)null;
            parameters = (IrcString[])null;
            if (message.Length < 2 || (int)message[0] != 1 || (int)message[message.Length - 1] != 1)
                return false;
            IrcString[] ircStringArray = message.Substring(1, message.Length - 2).Split((byte)32, 2);
            command = ircStringArray[0];
            rawParameter = ircStringArray.Length >= 2 ? ircStringArray[1] : IrcString.Empty;
            List<List<byte>> list = new List<List<byte>>();
            int index1 = 0;
            bool flag1 = false;
            bool flag2 = false;
            for (int index2 = 0; index2 < rawParameter.Length; ++index2)
            {
                byte num = rawParameter[index2];
                switch (num)
                {
                    case (byte)0:
                        break;
                    case (byte)1:
                        break;
                    case (byte)13:
                        break;
                    case (byte)10:
                        break;
                        //goto case (byte)0;
                    default:
                        byte? nullable1 = new byte?();
                        if (flag1)
                        {
                            nullable1 = new byte?(num);
                            flag1 = false;
                        }
                        else if ((int)num == 92)
                            flag1 = true;
                        else if ((int)num == 34)
                            flag2 = !flag2;
                        else if ((int)num == 32)
                        {
                            if (flag2)
                                nullable1 = new byte?(num);
                            else
                                ++index1;
                        }
                        else
                            nullable1 = new byte?(num);
                        byte? nullable2 = nullable1;
                        if ((nullable2.HasValue ? new int?((int)nullable2.GetValueOrDefault()) : new int?()).HasValue)
                        {
                            while (list.Count <= index1)
                                list.Add(new List<byte>());
                            list[index1].Add(nullable1.Value);
                            //goto case (byte)0;
                        }
                        break;
                        //else
                            //goto case (byte)0;
                }
            }
            parameters = Enumerable.ToArray<IrcString>(Enumerable.Select<List<byte>, IrcString>((IEnumerable<List<byte>>)list, (Func<List<byte>, IrcString>)(paramByte => new IrcString(paramByte.ToArray()))));
            return true;
        }

        protected virtual void OnCtcpCommandReceived(IrcIdentity sender, IrcString recipient, IrcString command, IrcString[] parameters, IrcString rawParameter)
        {
            switch ((string)command)
            {
                case "ACTION":
                    this.OnGotChatAction(new ChatMessageEventArgs(sender, recipient, rawParameter));
                    break;
                case "DCC":
                    if (parameters.Length < 1)
                        break;
                    this.OnDccCommandReceived(sender, recipient, parameters[0], Enumerable.ToArray<IrcString>(Enumerable.Skip<IrcString>((IEnumerable<IrcString>)parameters, 1)));
                    break;
                case "PING":
                    if (parameters.Length < 1 || IrcValidation.IsChannelName(recipient))
                        break;
                    this.CtcpReply(sender.Nickname, (IrcString)"PING", new IrcString[1]
          {
            parameters[0]
          }, 1 != 0);
                    break;
                case "TIME":
                    if (IrcValidation.IsChannelName(recipient))
                        break;
                    this.CtcpReply(sender.Nickname, (IrcString)"TIME", new IrcString[1]
          {
            (IrcString) this.CtcpTimeGetNow().ToString("ddd MMM dd HH:mm:ss yyyy", (IFormatProvider) DateTimeFormatInfo.InvariantInfo)
          }, 1 != 0);
                    break;
                case "VERSION":
                    if (IrcValidation.IsChannelName(recipient))
                        break;
                    this.CtcpReply(sender.Nickname, (IrcString)"VERSION", new IrcString[1]
          {
            this.ClientVersion
          }, 1 != 0);
                    break;
            }
        }

        protected virtual void OnCtcpReplyReceived(IrcIdentity sender, IrcString recipient, IrcString command, IrcString[] parameters, IrcString rawParameter)
        {
            switch ((string)command)
            {
                case "PING":
                    int result;
                    if (parameters.Length < 1 || !int.TryParse((string)parameters[0], out result))
                        break;
                    int timeDifference = this.CtcpPingGetTimeDifference(result, this.CtcpPingGetTimestamp());
                    this.OnGotPingReply(new PingReplyEventArgs(sender, timeDifference));
                    break;
            }
        }

        protected virtual void OnDccCommandReceived(IrcIdentity sender, IrcString recipient, IrcString command, IrcString[] parameters)
        {
        }

        protected virtual void OnIrcStatementReceived(IrcStatement statement)
        {
            IrcIdentity source = statement.Source;
            string str = (string)statement.Command;
            IList<IrcString> parameters1 = statement.Parameters;
            switch ((string)statement.Command)
            {
                case "NICK":
                    if (parameters1.Count < 1)
                        break;
                    this.OnGotNameChange(new NameChangeEventArgs(source, parameters1[0]));
                    break;
                case "INVITE":
                    if (parameters1.Count < 2)
                        break;
                    this.OnGotInvitation(new InvitationEventArgs(source, parameters1[0], parameters1[1]));
                    break;
                case "KICK":
                    if (parameters1.Count < 2)
                        break;
                    this.OnGotUserKicked(new KickEventArgs(source, parameters1[1], parameters1[0], parameters1.Count >= 3 ? parameters1[2] : (IrcString)null));
                    break;
                case "PRIVMSG":
                    if (parameters1.Count < 2)
                        break;
                    IrcString command1;
                    IrcString[] parameters2;
                    IrcString rawParameter1;
                    if (IrcClient.TryCtcpDecode(parameters1[1], out command1, out parameters2, out rawParameter1))
                    {
                        this.OnCtcpCommandReceived(source, parameters1[0], command1, parameters2, rawParameter1);
                        break;
                    }
                    else
                    {
                        this.OnGotMessage(new ChatMessageEventArgs(source, parameters1[0], parameters1[1]));
                        break;
                    }
                case "NOTICE":
                    if (parameters1.Count < 2)
                        break;
                    IrcString command2;
                    IrcString[] parameters3;
                    IrcString rawParameter2;
                    if (IrcClient.TryCtcpDecode(parameters1[1], out command2, out parameters3, out rawParameter2))
                    {
                        this.OnCtcpReplyReceived(source, parameters1[0], command2, parameters3, rawParameter2);
                        break;
                    }
                    else
                    {
                        this.OnGotNotice(new ChatMessageEventArgs(source, parameters1[0], parameters1[1]));
                        break;
                    }
                case "JOIN":
                    if (parameters1.Count < 1)
                        break;
                    this.OnGotJoinChannel(new JoinLeaveEventArgs(source, parameters1[0].Split((byte)44)));
                    break;
                case "PART":
                    if (parameters1.Count < 1)
                        break;
                    this.OnGotLeaveChannel(new JoinLeaveEventArgs(source, parameters1[0].Split((byte)44)));
                    break;
                case "QUIT":
                    this.OnGotUserQuit(new QuitEventArgs(source, parameters1.Count >= 1 ? parameters1[0] : (IrcString)null));
                    break;
                case "MODE":
                    if (parameters1.Count < 2)
                        break;
                    this.OnGotMode(new ModeEventArgs(source, parameters1[0], parameters1[1], Enumerable.ToArray<IrcString>(Enumerable.Skip<IrcString>((IEnumerable<IrcString>)parameters1, 2))));
                    break;
                case "PING":
                    if (parameters1.Count < 1)
                        break;
                    this.IrcCommand((IrcString)"PONG", new IrcString[1]
          {
            parameters1[0]
          });
                    break;
                case "001":
                    if (parameters1.Count < 1)
                        break;
                    this.OnGotWelcomeMessage(new SimpleMessageEventArgs(parameters1[0]));
                    break;
                case "375":
                    this.OnGotMotdBegin();
                    break;
                case "372":
                    if (parameters1.Count < 1)
                        break;
                    this.OnGotMotdText(new SimpleMessageEventArgs(parameters1[parameters1.Count - 1]));
                    break;
                case "376":
                    this.OnGotMotdEnd();
                    break;
                case "353":
                    if (parameters1.Count < 2)
                        break;
                    this.OnGotNameListReply(new NameListReplyEventArgs(parameters1[parameters1.Count - 2], parameters1[parameters1.Count - 1].Split((byte)32)));
                    break;
                case "366":
                    if (parameters1.Count < 1)
                        break;
                    this.OnGotNameListEnd(new NameListEndEventArgs(parameters1[0]));
                    break;
                case "321":
                    this.OnGotChannelListBegin();
                    break;
                case "322":
                    int result;
                    if (parameters1.Count < 4 || !int.TryParse((string)parameters1[2], out result))
                        break;
                    this.OnGotChannelListEntry(new ChannelListEntryEventArgs(parameters1[1], result, parameters1[parameters1.Count - 1]));
                    break;
                case "323":
                    this.OnGotChannelListEnd();
                    break;
                case "331":
                    if (parameters1.Count < 3)
                        break;
                    this.OnGotChannelTopicChange(new ChannelTopicChangeEventArgs(parameters1[1], IrcString.Empty));
                    break;
                case "332":
                    if (parameters1.Count < 3)
                        break;
                    this.OnGotChannelTopicChange(new ChannelTopicChangeEventArgs(parameters1[1], parameters1[2]));
                    break;
                default:
                    if (statement.ReplyCode < (IrcReplyCode)400 || statement.ReplyCode >= (IrcReplyCode)600)
                        break;
                    this.OnGotIrcError(new IrcErrorEventArgs(statement.ReplyCode, statement));
                    break;
            }
        }

        private class Context
        {
            public Thread ReceiverThread;
            public ManualResetEvent StartEvent;
            public Stream Stream;
            public SynchronizationContext SynchronizationContext;
        }
    }
}
