﻿using BotLeecher.Service;
using FirstFloor.ModernUI.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WPFGenerics;

namespace BotLeecherWPF.ViewModel
{
    [Export]
    public class MainViewModel : ViewModelBase
    {
        private BotMediator BotMediator;
        private EventMediatorService Service;
        private ObservableCollection<string> serverList = new ObservableCollection<string>();
        private ObservableCollection<string> channelList = new ObservableCollection<string>();
        private SynchronizationContext context;
        private string log = "";
        private TaskbarIcon tbi;

        public string Server { get; set; }
        public string Channel { get; set; }
        public string User { get; set; }
        public string Log { get { return log; } set { SetProperty(ref log, value); } }

        private ObservableCollection<string> userList = new ObservableCollection<string>();
        private ICommand connectCommand;
        private ICommand listCommand;

        private ObservableCollection<ItemViewModel> bots = new ObservableCollection<ItemViewModel>();

        public ICommand ConnectCommand
        {
            get
            {
                return connectCommand ?? (connectCommand = new CommandHandler(Connect, true));
            }
        }

        public ICommand ListCommand
        {
            get
            {
                return listCommand ?? (listCommand = new CommandHandler(GetList, true));
            }
        }

        public ObservableCollection<ItemViewModel> Bots
        {
            get
            {
                return bots;
            }
        }

        public ObservableCollection<string> ServerList
        {
            get
            {
                return serverList;
            }
        }

        public ObservableCollection<string> ChannelList
        {
            get
            {
                return channelList;
            }
        }

        public ObservableCollection<string> UserList
        {
            get
            {
                return userList;
            }
        }

        [ImportingConstructor]
        public MainViewModel(BotMediator botMediator, EventMediatorService service)
        {
            this.BotMediator = botMediator;
            this.Service = service;
            foreach (string server in this.BotMediator.GetServers())
            {
                serverList.Add(server);
            }
            foreach (string channel in this.BotMediator.GetChannels())
            {
                channelList.Add(channel);
            }
            User = "";

            Service.UserListEvent += OnUserList;
            Service.MessageEvent += OnMessage;
            Service.PackEvent += OnPack;
            Service.TransferStatusEvent += OnTransferStatus;
            tbi = new TaskbarIcon();

            tbi.Icon = new Icon(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("BotLeecherWPF.Resources.PerfCenterCpl.ico"));
            tbi.Visibility = System.Windows.Visibility.Visible;

        }

        public void GetList()
        {
            GetList(true);
        }

        public void GetList(bool refresh)
        {
            GetList(User, refresh);
        }

        public void GetList(string name, bool refresh)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                GetItem(name).GetList(refresh);
            }
        }

        public void Connect()
        {
            UserList.Clear();
            BotMediator.Connect(Server, Channel);
        }

        public ItemViewModel GetItem(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            ItemViewModel item = null;
            foreach (var bot in bots)
            {
                if (bot.Name == name)
                {
                    item = bot;
                    break;
                }
            }
            if (item == null)
            {
                item = new ItemViewModel(name, BotMediator, Service);
                bots.Add(item);
            }

            return item;
        }


        private void OnTransferStatus(object sender, Event.TransferStatusEventArgs e)
        {
            var item = GetItem(e.BotName);
            if (item != null)
            {
                item.State.Name = e.FileName;
                item.State.Progress = e.Completion;
                
                if (TaskbarManager.IsPlatformSupported)
                {
                    TaskbarManager.Instance.SetProgressValue(e.Completion, 100);
                }
            }
        }

        private void OnPack(object sender, Event.PackEventArgs e)
        {
            var item = GetItem(e.BotName);
            if (item != null)
            {
                item.SetPacks(e.PackList);
            }
        }

        private void OnMessage(object sender, Event.MessageEventArgs e)
        {
            context = context ?? SynchronizationContext.Current;
            context.Send(x =>
            {
                Event.MessageEventArgs msg = (Event.MessageEventArgs)x;
                Log = DateTime.Now.ToLongTimeString() + ": " + msg.Message + "\n" + Log;
                BalloonIcon icon;
                if (msg.Type == MessageType.ERROR)
                {
                    icon = BalloonIcon.Error;
                }
                else if (msg.Type == MessageType.DOWNLOAD)
                {
                    icon = BalloonIcon.None;
                }
                else
                {
                    icon = BalloonIcon.Info;
                }
                tbi.ShowBalloonTip(Enum.GetName(typeof(MessageType), msg.Type), Log.Substring(0, Math.Min(Log.Length, 500)), icon);
            }, e);
        }

        private void OnUserList(object sender, Event.UserListEventArgs e)
        {
            var tmpList = new List<string>();
            foreach (var user in e.Users)
            {
                if (!UserList.Contains(user))
                {
                    tmpList.Add(user);
                }
            }
            IList<string> sortedList = tmpList.OrderBy(o => o).ToList();
            context = context ?? SynchronizationContext.Current;
            context.Send(x =>
            {
                IList<string> list = (IList<string>)x;
                UserList.Clear();
                foreach (var sortedItem in list)
                    UserList.Add(sortedItem);
            }, sortedList);
        }
    }
}
