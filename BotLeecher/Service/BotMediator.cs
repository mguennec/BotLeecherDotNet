using BotLeecher.Entities;
using BotLeecher.Enums;
using BotLeecher.Model;
using ircsharp;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace BotLeecher.Service
{
    [Export]
    public class BotMediator : IrcConnectionListener, TextWriter, BotListener {
        private static readonly ILog LOGGER = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IrcConnection _ircConnection;
        private NicknameProvider NickProvider;
        private BotLeecherFactory BotLeecherFactory;
        private Settings Settings;

        public string Server { get; set; }
        public string Channel { get; set; }
        private IrcConnection IrcConnection
        {
            get
            {
                return _ircConnection ?? (_ircConnection = new IrcConnection(NickProvider, BotLeecherFactory, this));
            }
            set
            {
                _ircConnection = value;
            }
        }
        public IDictionary<string, User> Users { get; set; }
        private EventMediatorService Service;
        private bool ListAsked;

        [ImportingConstructor]
        public BotMediator(Settings settings, EventMediatorService service, NicknameProvider nickProvider, BotLeecherFactory botLeecherFactory) {
            this.NickProvider = nickProvider;
            this.BotLeecherFactory = botLeecherFactory;
            this.Settings = settings;
            this.Service = service;
            this.Users = new Dictionary<string, User>();
            var timer = new Timer(10000);
            timer.Elapsed += VerifyConnection;
            timer.Start();
        }

        private void VerifyConnection(object sender, ElapsedEventArgs e)
        {
            IrcConnection.Ping();
        }

        private void EventHandling() {
            //IrcConnection.GotNameListReply += OnUserList;
            //IrcConnection.GotMessage += OnMessage;
        }
        
        public void WriteText(string text, MessageType type) {
            Service.SendMessage(text, type);
        }

        public void WriteText(string text) {
            WriteText(text, MessageType.INFO);
        }

        public void WriteError(string text) {
            WriteText(text, MessageType.ERROR);
        }

        public void LaunchPlayer(string fileName)
        {
            Process.Start(GetPlayer(), GetPlayerOptions() + " \"" + GetSaveDir() + Path.DirectorySeparatorChar + fileName + "\"");
        }

        public void Disconnected(bool reconnect = false) {
            if (IrcConnection != null)
            {
                Service.SendUserList(new List<string>());
                Users.Clear();
                IrcConnection.Disconnect();
                IrcConnection = null;
            }
            if (reconnect)
            {
                WriteText("Disconnected");
                ReConnect();
            }
        }

        public List<BotLeecher> GetAllBots() {
            return IrcConnection.GetAllBots();
        }

        public void UserListLoaded(string channel, IList<User> users) {
            if (ListAsked)
            {
                Users.Clear();
                ListAsked = false;
            }
            foreach (User user in users)
            {
                Users[user.Nick] = user;
            }
            WriteText("User list received for " + channel);
            Service.SendUserList(new List<string>(Users.Keys));
        }

        /**
         *
         */
        public void OnDisconnect() {
            LOGGER.Info("DISCONNECT:\tDisconnected from server");
            Disconnected(true);
        }

        public BotMediator() {
            //redirectOutputStreams();
        }

        /**
         * Connects to the irc network
         */
        public void Connect(string server, string channel, bool reconnect = false)
        {
            if (IrcConnection != null) {
                Disconnected(reconnect);
            }
            if (!reconnect)
            {
                if (!GetServers().Contains(server))
                {
                    AddServer(server);
                }
                if (!GetChannels().Contains(channel))
                {
                    AddChannel(channel);
                }
                this.Server = server;
                this.Channel = channel;
                EventHandling();
                ListAsked = true;
                WriteText("Connection on " + this.Server + ":" + this.Channel);
                IrcConnection.Connect();
            }
        }

        public void ReConnect()
        {
            Connect(this.Server, this.Channel);
        }

        public void GetList(string user, bool refresh)
        {
            IrcConnection.Ping();
            BotLeecher botLeecher = IrcConnection.GetBotLeecher(user);
            if (botLeecher == null) {
                CreateLeecher(user);
            } else {
                if (refresh || botLeecher.PackList == null) {
                    botLeecher.RequestPackList();
                } else {
                    PackListLoaded(user, botLeecher.PackList.Packs);
                }
            }
        }

        public IList<Pack> GetCurrentPackList(string user) {
            IList<Pack> packs = new List<Pack>();
            BotLeecher botLeecher = IrcConnection.GetBotLeecher(user);
            if (botLeecher != null) {
                PackList packList = botLeecher.PackList;
                if (packList != null) {
                    foreach (Pack pack in packList.Packs) {
                        packs.Add(pack);
                    }
                }
            }
            return packs;
        }

        public void GetPack(string user, int pack)
        {
            IrcConnection.Ping();
            BotLeecher botLeecher = IrcConnection.GetBotLeecher(user);
            if (botLeecher != null) {
                WriteText(user + " : Sending Request for pack #" + pack, MessageType.REQUEST);
                botLeecher.RequestPack(pack);
            }
        }

        public int GetProgress(string user) {
            BotLeecher botLeecher = IrcConnection == null ? null : IrcConnection.GetBotLeecher(user);
            int progress = 0;
            if (botLeecher != null && botLeecher.CurrentTransfer != null)
            {
                progress = (int)(((double)botLeecher.GetCurrentState() / (double)botLeecher.FileSize) * 100);
            }
            return progress;
        }

        public long GetTransferRate(string user) {
            BotLeecher botLeecher = IrcConnection == null ? null : IrcConnection.GetBotLeecher(user);
            long rate;
            if (botLeecher != null && botLeecher.CurrentTransfer != null) {
                rate = botLeecher.GetTransfertRate();
            } else {
                rate = 0;
            }
            return rate;
        }

        public IList<String> GetServers() {
            return Settings.Get(SettingProperty.PROP_SERVER).Value;
        }

        public void AddServer(string server) {
            Setting setting = Settings.Get(SettingProperty.PROP_SERVER);
            setting.Value.Add(server);
            Settings.Save(setting);
        }

        public IList<String> GetChannels() {
            return Settings.Get(SettingProperty.PROP_CHANNEL).Value;
        }

        public void AddChannel(string channel) {
            Setting setting = Settings.Get(SettingProperty.PROP_CHANNEL);
            setting.Value.Add(channel);
            Settings.Save(setting);
        }

        public String GetSaveDir()
        {
            return Settings.Get(SettingProperty.PROP_SAVEFOLDER).GetFirstValue();
        }

        public void SetSaveDir(string path)
        {
            Settings.Save(new Setting(SettingProperty.PROP_SAVEFOLDER, path));
        }

        public String GetPlayer()
        {
            return Settings.Get(SettingProperty.PROP_MEDIAPLAYER).GetFirstValue();
        }

        public void SetPlayer(string path)
        {
            Settings.Save(new Setting(SettingProperty.PROP_MEDIAPLAYER, path));
        }

        public String GetPlayerOptions()
        {
            return Settings.Get(SettingProperty.PROP_MEDIAPLAYER_OPTIONS).GetFirstValue();
        }

        public void SetPlayerOptions(string options)
        {
            Settings.Save(new Setting(SettingProperty.PROP_MEDIAPLAYER_OPTIONS, options));
        }

        public IList<String> GetNicks() {
            return Settings.Get(SettingProperty.PROP_NICKS).Value;
        }

        public void SetNicks(IList<string> nicks) {
            Settings.Save(new Setting(SettingProperty.PROP_NICKS, nicks));
        }

        public IList<String> GetKeywords() {
            return Settings.Get(SettingProperty.PROP_KEYWORDS).Value;
        }

        public void SetKeywords(IList<string> keywords) {
            Settings.Save(new Setting(SettingProperty.PROP_KEYWORDS, keywords));
        }

        public String GetCurrentFile(string user) {
            BotLeecher botLeecher = IrcConnection == null ? null : IrcConnection.GetBotLeecher(user);
            string file;
            if (botLeecher != null && botLeecher.CurrentTransfer != null && botLeecher.FileName != null) {
                file = botLeecher.FileName;
            } else {
                file = "";
            }
            return file;
        }

        public void Cancel(string user) {
            BotLeecher botLeecher = IrcConnection == null ? null : IrcConnection.GetBotLeecher(user);
            if (botLeecher != null) {
                botLeecher.Cancel();
            }
        }

        public void CreateLeecher(string user) {
            if (Users.ContainsKey(user)) {
                IrcConnection.MakeLeecher(Users[user]);
            }
        }

        public void RemoveLeecher(string user) {
            if (Users.ContainsKey(user))
            {
                IrcConnection.RemoveLeecher(user);
            }
        }

        public void PackListLoaded(string botName, IList<Pack> packList) {
            Service.SendPack(botName, packList);
        }

        public void UpdateStatus(string botName, string fileName, int completion) {
            Service.SendTransferStatus(botName, fileName, completion);
        }

        public void Failure(User bot, string fileName)
        {
            Service.SendTransferFailed(bot.Nick, fileName);
        }

        public void Complete(User bot, string fileName)
        {
            Service.SendTransferComplete(bot.Nick, fileName);
            if (!fileName.ToLower().EndsWith("txt"))
            {
                LaunchPlayer(fileName);
            }
        }

        public void Beginning(User botName, string fileName)
        {
            Service.SendMessage("Starting download:" + fileName, MessageType.DOWNLOAD);
        }

        public void PackListLoaded(User botName, IList<Pack> packList)
        {
            Service.SendPack(botName.Nick, packList);
        }

        public void UpdateStatus(User botName, string fileName, int completion)
        {
            Service.SendTransferStatus(botName.Nick, fileName, completion);
        }


        //private void OnMessage(object sender, ChatMessageEventArgs e)
        //{
        //    
        //}

        public void OnMessage(User user, string message)
        {
            if (user != null && message != null)
            {
                foreach (string keyword in Settings.Get(SettingProperty.PROP_KEYWORDS).Value)
                {
                    if (message.ToLower().Contains(keyword.ToLower()))
                    {
                        WriteText(user.Nick + " : " + message);
                        break;
                    }
                }
            }
        }
    }
}
