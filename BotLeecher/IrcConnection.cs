using BotLeecher.Service;
using BotLeecher.Tools;
using ircsharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher
{
    public class IrcConnection : Connection {

        private IList<IrcConnectionListener> Listeners;
        //private PropertyChangeSupport propertyChangeSupport;

        private IDictionary<string, BotLeecher> Leechers;

        private BotLeecherFactory BotLeecherFactory;

        private BotMediator Mediator;
        private NicknameProvider NickProvider;
        public bool IsConnected;

        public event EventHandler OnConnectionEstablished;
        

        /**
         * Creates a new instance of Main
         */
        public IrcConnection(NicknameProvider nickProvider, BotLeecherFactory botLeecherFactory, BotMediator mediator)
            : base(nickProvider.GetNickName(), nickProvider.GetNickName())
        {
            /*super(new Configuration.Builder()
                    .setLogin(nickProvider.getNickName()).setName(nickProvider.getNickName())
                    .setFinger(nickProvider.getNickName()).setVersion("xxx").setAutoNickChange(true)
                    .addListener(mediator).setServerHostname(mediator.getServer()).addAutoJoinChannel(mediator.getChannel())
                    .buildConfiguration());*/
            this.BotLeecherFactory = botLeecherFactory;
            this.NickProvider = nickProvider;
            this.Leechers = new ConcurrentDictionary<string, BotLeecher>();
            this.Listeners = new List<IrcConnectionListener>();
            this.Listeners.Add(mediator);
            this.IsConnected = false;
            this.Mediator = mediator;
            base.Connected += JoinChannel;
            base.Disconnected += Disconnection;
        }


        /**
         * @param user
         * @return
         */
        public BotLeecher MakeLeecher(User user)
        {
            BotLeecher leecher = null;
            if (user != null)
            {
                leecher = BotLeecherFactory.GetBotLeecher(user, this);
                Leechers.Add(user.Nick, leecher);
                leecher.AddListener(Mediator);
                leecher.Start();
            }
            return leecher;
        }

        /**
         * @param user
         * @return
         */
        public BotLeecher MakeLeecher(string user)
        {
            return MakeLeecher(Users.GetUser(user));
        }
        
        public void RemoveLeecher(string user) {
            BotLeecher leecher = Leechers[user];
            if (leecher != null) {
                Leechers.Remove(user);
            }
        }

        public void Disconnection(object sender, EventArgs e)
        {
            if (this.IsConnected)
            {
                this.IsConnected = false;
                Mediator.OnDisconnect();
            }
        }

        public void Connect()
        {
            base.Connect(Mediator.Server, 6667);
        }
        private void JoinChannel(object sender, EventArgs e)
        {
            base.Channels.JoinComplete += OnChannelJoined;
            base.Channels.Join(Mediator.Channel);
        }

        public void Ping()
        {
            Server.RawSend("ping " + Mediator.Channel);
            var waiter = new IrcEventWaiter(Server, "PingPong");
            waiter.WaitForEvent(TimeSpan.FromSeconds(30));
        }

        private void OnChannelJoined(ircsharp.Connection sender, JoinCompleteEventArgs e)
        {
            var channel = e.Channel;
            var list = new List<User>();
            var enumerator = channel.Users.GetEnumerator();
            while (enumerator.MoveNext())
            {
                list.Add(((ChannelUser) enumerator.Current).User);
            }
            Mediator.UserListLoaded(channel.Name, list);
            GetMessageReciever(channel.NetworkIdentifier).RecievedMessage += OnMessage;
            this.IsConnected = true;
            if (OnConnectionEstablished != null) {
                OnConnectionEstablished(this, new EventArgs());
            }
        }

        private void OnMessage(MessageReciever sender, MessageEventArgs e)
        {
            foreach (var listener in Listeners) {
                listener.OnMessage(e.User, e.Message);
            }
        }


        /**
         * @param botName
         * @return
         */
        public BotLeecher GetBotLeecher(string botName) {
            return Leechers.ContainsKey(botName) ? Leechers[botName] : MakeLeecher(botName);
        }

        public List<BotLeecher> GetAllBots() {
            return new List<BotLeecher>(Leechers.Values);
        }

        /**
         * @param listener
         */
        public void RemoveBotListener(IrcConnectionListener listener) {
            Listeners.Remove(listener);
        }
    }
}
