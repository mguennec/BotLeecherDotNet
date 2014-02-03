using BotLeecher.NetIrc;
using BotLeecher.Service;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher
{
    public class IrcConnection : NetIrc.IrcClient {

        private IList<IrcConnectionListener> Listeners;
        //private PropertyChangeSupport propertyChangeSupport;

        private IDictionary<string, BotLeecher> Leechers;

        private BotLeecherFactory BotLeecherFactory;

        private BotMediator Mediator;
        private NicknameProvider NickProvider;
        

        /**
         * Creates a new instance of Main
         */
        public IrcConnection(NicknameProvider nickProvider, BotLeecherFactory botLeecherFactory, BotMediator mediator) : base() {
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
            this.Mediator = mediator;
            base.Connected += JoinChannel;
            base.Closed += Disconnected;
        }

        
        /**
         * @param user
         * @return
         */
        public BotLeecher MakeLeecher(IrcString user) {
            BotLeecher leecher = BotLeecherFactory.getBotLeecher(user, this);
            Leechers.Add(user, leecher);
            leecher.AddListener(Mediator);
            leecher.Start();
            return leecher;
        }
        
        public void RemoveLeecher(string user) {
            BotLeecher leecher = Leechers[user];
            if (leecher != null) {
                leecher.Stop();
                Leechers.Remove(user);
            }
        }

        public void Disconnected(object sender, EventArgs e)
        {
            foreach (string user in Leechers.Keys) {
                RemoveLeecher(user);
            }
            Mediator.OnDisconnect();
        }

        public void Connect()
        {
            Connect(Mediator.Server);
            LogIn(NickProvider.GetNickName(), NickProvider.GetNickName(), NickProvider.GetNickName());
            Join(Mediator.Channel);
            IrcCommand("NAMES", Mediator.Channel);
        }
        private void JoinChannel(object sender, EventArgs e)
        {
            //Join(Mediator.Channel);
        }


        /**
         * @param botName
         * @return
         */
        public BotLeecher GetBotLeecher(String botName) {
            return Leechers.ContainsKey(botName)? Leechers[botName] : null;
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

        protected override void OnDccCommandReceived(IrcIdentity sender, IrcString recipient, IrcString command, IrcString[] parameters)
        {
            if (command.ToString() == "SEND")
            {
                Mediator.OnIncomingFileTransfer(sender, recipient, command, parameters);
            }
        }

    }
}
