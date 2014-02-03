using BotLeecher.Entities;
using BotLeecher.Enums;
using BotLeecher.Model;
using BotLeecher.NetIrc;
using BotLeecher.NetIrc.Events;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher.Service
{
    [Export]
    public class BotMediator : IrcConnectionListener, TextWriter, BotListener {
        private static readonly ILog LOGGER = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IrcConnection IrcConnection;
        private NicknameProvider NickProvider;
        private BotLeecherFactory BotLeecherFactory;
        private Settings Settings;

        public string Server { get; set; }
        public string Channel { get; set; }
        public IList<string> Users { get; set; }
        private EventMediatorService Service;
        private bool ListAsked;

        [ImportingConstructor]
        public BotMediator(Settings settings, EventMediatorService service, NicknameProvider nickProvider, BotLeecherFactory botLeecherFactory) {
            this.NickProvider = nickProvider;
            this.BotLeecherFactory = botLeecherFactory;
            this.Settings = settings;
            this.Service = service;
            this.Users = new List<string>();
        }    

        private void EventHandling() {
            IrcConnection.GotNameListReply += OnUserList;
            IrcConnection.GotMessage += OnMessage;
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

        public void Disconnected() {
            if (IrcConnection != null) {
                var c = IrcConnection;
                IrcConnection = null;
                if (c.IsConnected) {
                    c.Close();
                }
                Users.Clear();
                Service.SendUserList(new List<string>());
                WriteText("Disconnected");
            }
        }

        public List<BotLeecher> GetAllBots() {
            return IrcConnection == null ? new List<BotLeecher>() : IrcConnection.GetAllBots();
        }

        public void UserListLoaded(string channel, IList<IrcString> users) {
            if (ListAsked)
            {
                Users.Clear();
                ListAsked = false;
            }
            foreach (IrcString user in users) {
                Users.Add(user.ToString().TrimStart('%', '+', '@'));
            }
            Service.SendUserList(Users);
        }

        public void OnUserList(object sender, NameListReplyEventArgs e)
        {
            IList<IrcString> list = new List<IrcString>(e.GetNameList()).OrderBy(o => o.ToString()).ToList<IrcString>();
            UserListLoaded(e.Channel, list);
        }

        /**
         *
         */
        public void OnDisconnect() {
            LOGGER.Info("DISCONNECT:\tDisconnected from server");
            Disconnected();
        }

        public BotMediator() {
            //redirectOutputStreams();
        }

        private void OnMessage(object sender, ChatMessageEventArgs e)
        {
 	        string message = e.Message;
            WriteText(e.Sender + " : " + message);
            if (message != null) {
                foreach (string keyword in Settings.Get(SettingProperty.PROP_KEYWORDS).Value) {
                    if (message.ToLower().Contains(keyword.ToLower())) {
                        WriteText(e.Sender + " : " + message);
                        break;
                    }
                }
            }
        }

        public void OnIncomingFileTransfer(IrcIdentity sender, IrcString recipient, IrcString command, IrcString[] parameters) {
            var senderName = sender.Nickname;
            if (IrcConnection != null) {
                WriteText(senderName + " : Downloading " + parameters[0], MessageType.DOWNLOAD);
                try {
                    IrcConnection.GetBotLeecher(senderName).OnIncomingFileTransfer(sender, parameters);
                    WriteText(senderName + " : Download complete " + parameters[0], MessageType.DOWNLOAD);
                } catch (Exception e) {
                    WriteError(e.Message);
                    WriteError(senderName + " : Download error " + parameters[0]);
                }
            }
        }

        /**
         * Connects to the irc network
         */
        public void Connect(string server, string channel) {
            if (IrcConnection != null) {
                Disconnected();
            }
            if (!GetServers().Contains(server)) {
                AddServer(server);
            }
            if (!GetChannels().Contains(channel)) {
                AddChannel(channel);
            }
            this.Server = server;
            this.Channel = channel;
            IrcConnection = new IrcConnection(NickProvider, BotLeecherFactory, this);
            EventHandling();
            ListAsked = true;
            IrcConnection.Connect();
        }

        public void GetList(string user, bool refresh) {
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

        public void GetPack(string user, int pack) {
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

        public String GetSaveDir() {
            return Settings.Get(SettingProperty.PROP_SAVEFOLDER).GetFirstValue();
        }

        public void SetSaveDir(string path) {
            Settings.Save(new Setting(SettingProperty.PROP_SAVEFOLDER, path));
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
            String file;
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
            if (Users.Contains(user)) {
                IrcConnection.MakeLeecher(user);
            }
        }

        public void RemoveLeecher(string user) {
            if (Users.Contains(user)) {
                IrcConnection.RemoveLeecher(user);
            }
        }

        public void PackListLoaded(string botName, IList<Pack> packList) {
            Service.SendPack(botName, packList);
        }

        public void UpdateStatus(string botName, string fileName, int completion) {
            Service.SendTransferStatus(botName, fileName, completion);
        }
    }
}
