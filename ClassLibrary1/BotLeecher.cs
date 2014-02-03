using BotLeecher.Enums;
using BotLeecher.Model;
using BotLeecher.Service;
using BotLeecher.Tools;
using log4net;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using BotLeecher.NetIrc;
using System.Net.NetworkInformation;

namespace BotLeecher
{
    public class BotLeecher
    {
        public static readonly ILog LOGGER = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Task Task;
        public readonly LeecherQueue Queue;
        public Settings Settings { get; set; }
        public PackListReader PackListReader { get; set; }
        public IrcString BotUser { get; set; }
        public string Description { get; set; }
        public IrcConnection Connection { get; set; }
        public bool Leeching { get; set; }
        public bool Downloading { get; set; }
        public bool ListRequested { get; set; }
        public int Counter { get; set; }
        public ReceiveFileTransfer CurrentTransfer { get; set; }
        public long FileSize { get; set; }
        public string ListFile { get; set; }
        public IList<BotListener> Listeners { get; set; }
        public DateTime StartTime { get; set; }
        public PackList PackList { get; set; }
        public String FileName { get; set; }

        public BotLeecher(IrcString user, IrcConnection connection, Settings settings, PackListReader packListReader) {
        
            this.Settings = settings;
            this.PackListReader = packListReader;

            this.Counter = 1;
            this.BotUser = user;
            this.Connection = connection;
            //this.CurrentTransfer = null;
            this.Leeching = false;
            this.ListRequested = false;
            this.Description = "";
            this.Listeners = new List<BotListener>();
            this.Queue = new LeecherQueue(this);
        }

        public void RunQueue() {
            Task = new Task(() => {
                Queue.Run();
                Task = null;
            });
            Task.Start();
        }

        public void Start() {
            RequestPackList();
            RunQueue();
        }

        public void RequestPackList() {
            Queue.Add(1);
        }

        public void Cancel() {
            Queue.Cancel();
        }


        /**
         * @param transfer
         */
        public void OnIncomingFileTransfer(IrcIdentity sender, IrcString[] parameters) {
            int port;
            int.TryParse(parameters[2], out port);
            int localPort;
            //Socket socket = GetSocket(parameters[1], port);
            TcpListener listener = GetSocket(out localPort);
            Connection.Message(BotUser, "DCC ACCEPT " + parameters[0] + " " + localPort.ToString());
            Task<Socket> userSocket = listener.AcceptSocketAsync();
            //socket.Close();
            long size;
            long.TryParse(parameters[3], out size);
            FileSize = size;
            CurrentTransfer = new ReceiveFileTransfer(userSocket, sender.ToString(), GetFileName(parameters[0]), 0);
            Queue.OnIncomingFileTransfer();
        }

        private string GetFileName(string fileName)
        {
            FileName = fileName;
 	        var path = Settings.Get(SettingProperty.PROP_SAVEFOLDER).GetFirstValue();
            return path + Path.DirectorySeparatorChar + FileName;
        }

        private TcpListener GetSocket(out int localPort) {
            TcpListener s = null;
            localPort = 0;
            IPAddress address = null;
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var nic in nics) {
                if (address != null)
                {
                    break;
                }
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (var prop in nic.GetIPProperties().UnicastAddresses)
                    {
                        if (prop.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            address = prop.Address;
                            break;
                        }
                    }
                }
            }
            if (address == null) {
                return s;
            }
            foreach (int port in IrcClient.Ports)
            {
                try {
                    s = new TcpListener(address, port);
                    localPort = port;
                    s.Start();
                    break;
                } catch (SocketException e) {
                    // Nothing
                }
            }
            return s;
        }

        private void DownloadFinished(string fileName) {
            ChangeState(fileName, new PackStatus(PackStatus.Status.DOWNLOADED));
            LOGGER.Info("FINISHED:\t Transfer finished for " + fileName);
        }

        /**
         * @param listener
         */
        public void AddListener(BotListener listener) {
            Listeners.Add(listener);
        }

        public void RequestPack(int nr) {
            Queue.Add(nr);
            ChangeState(nr, new PackStatus(PackStatus.Status.QUEUED));
        }

        private void FireListEvent() {
            foreach (BotListener listener in Listeners) {
                listener.PackListLoaded(BotUser, PackList.Packs);
            }
        }

        private void FireStatusEvent() {
            foreach (BotListener listener in Listeners) {
                listener.UpdateStatus(BotUser, FileName, this.GetProgress());
            }
        }
        
        public long GetTransfertRate() {
            long rate;
            if (CurrentTransfer == null) {
                rate = 0;
            } else {
                long currentData = CurrentTransfer == null ? 0 : CurrentTransfer.BytesTransfered;
                long diff = (new DateTime().Ticks - StartTime.Ticks) / 1000;
                rate = currentData / diff;
            }
            return rate;
        }
        public long GetCurrentState() {
            long state = 0;
            if (Downloading && CurrentTransfer != null) {
                state = CurrentTransfer.BytesTransfered;
            }

            return state;
        }

        public void Stop() {
            Queue.Stop();
            
        }

        public void ChangeState(string name, PackStatus status) {
            if (PackList != null) {
                ChangeState(PackList.GetByName(name), status);
            }
        }

        public void ChangeState(int nb, PackStatus status) {
            if (PackList != null) {
                ChangeState(PackList.GetByNumber(nb), status);
            }
        }

        public void ChangeState(Pack pack, PackStatus status) {
            if (pack != null && status != null) {
                pack.Status = status;
                FireListEvent();
            }
        }

        public int GetProgress() {
            int progress;
            if (CurrentTransfer != null) {
                progress = (int) (((double) GetCurrentState() / (double) FileSize) * 100);
            } else {
                progress = 0;
            }
            return progress;
        }

        public class LeecherQueue {

            private bool Working = true;
            private bool Canceled = false;
            private System.Timers.Timer Timer;
            private BlockingCollection<int> InternalQueue = new BlockingCollection<int>();
            private BotLeecher BotLeecher;

            public LeecherQueue(BotLeecher botLeecher) {
                this.BotLeecher = botLeecher;
            }

            public void Stop() {
                Working = false;
                Timer.Dispose();
                Timer = null;
            }

            public bool Add(int nr) {
                bool retVal;
                if (InternalQueue.Contains(nr)) {
                    retVal = false;
                } else {
                    InternalQueue.Add(nr);
                    retVal = true;
                }
                return retVal;
            }

            public void Run() {
                while (Working) {
                    if (Timer == null) {
                        StartWatcherThread();
                    }
                    int nr = InternalQueue.Take();
                    AskPack(nr);
                }
            }

            private void AskPack(int nr) {
                if (Canceled) {
                    Canceled = false;
                    BotLeecher.ChangeState(nr, new PackStatus(PackStatus.Status.AVAILABLE));
                } else {
                    if (1 == nr) {
                        BotLeecher.ListRequested = true;
                    }
                    BotLeecher.Connection.Message(BotLeecher.BotUser, "XDCC SEND " + nr);
                }
            }

            /**
             * @param transfer
             */
            public void OnIncomingFileTransfer() {
                if (BotLeecher.ListRequested) {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        
                        BotLeecher.CurrentTransfer.TransferToString(sb);
                        BotLeecher.LOGGER.Info("LIST:\t List received for " + BotLeecher.BotUser);
                        BotLeecher.ListRequested = false;
                        BotLeecher.PackList = BotLeecher.PackListReader.ReadPacks(sb);
                        foreach (String message in BotLeecher.PackList.Messages) {
                            BotLeecher.Description += message + "\n";
                        }
                        BotLeecher.FireListEvent();
                    } catch (IOException ex) {
                        LOGGER.Error("Error while receiving file!", ex);
                    }
                } else {
                    BotLeecher.ChangeState(BotLeecher.FileName, new PackStatus(PackStatus.Status.DOWNLOADING));
                    LOGGER.Info("INCOMING:\t" + BotLeecher.CurrentTransfer.File + " " +
                            BotLeecher.FileSize + " bytes");

                    //if file exists cut one 8bytes off to make transfer go on
                    if (File.Exists(BotLeecher.CurrentTransfer.File) && (BotLeecher.FileSize == new FileInfo(BotLeecher.CurrentTransfer.File).Length)) {
                        LOGGER.Info("EXISTS:\t try to close connection");

                        //FileImageInputStream fis = new FileInputStream
                    } else {
                        LOGGER.Info("SAVING TO:\t" + BotLeecher.CurrentTransfer.File);
                        try {
                            BotLeecher.Downloading = true;
                            BotLeecher.StartTime = new DateTime();
                            BotLeecher.CurrentTransfer.Transfer();
                            BotLeecher.DownloadFinished(BotLeecher.CurrentTransfer.File);
                        } catch (IOException e) {
                            BotLeecher.ChangeState(BotLeecher.CurrentTransfer.File, new PackStatus(PackStatus.Status.AVAILABLE));
                            File.Delete(BotLeecher.CurrentTransfer.File);
                            LOGGER.Error(e.Message, e);
                        }

                        BotLeecher.Downloading = false;
                        BotLeecher.CurrentTransfer = null;
                        BotLeecher.FileName = null;
                        BotLeecher.FireStatusEvent();
                    }
                }
            }

            public void Cancel() {
                Canceled = true;
                IList<int> list = InternalQueue.ToList();
                ClearQueue();
                foreach (int id in list) {
                    BotLeecher.ChangeState(id, new PackStatus(PackStatus.Status.AVAILABLE));
                }
                BotLeecher.Connection.Message(BotLeecher.BotUser, "XDCC CANCEL");
            }

            private void ClearQueue()
            {
                while (InternalQueue.Count > 0)
                {
                    int item;
                    InternalQueue.TryTake(out item);
                }
            }
            
            private void Watch(object sender, ElapsedEventArgs e)
            {
 	            if (Working) {
                    if (BotLeecher.CurrentTransfer != null) {
                        BotLeecher.FireStatusEvent();
                    }
                }
            } 
            private void StartWatcherThread() {
                if (Timer == null) {
                    Timer = new System.Timers.Timer(5000);
                    Timer.Elapsed += new ElapsedEventHandler(Watch);
                    Timer.Enabled = true;
                }
            }

        }
    }
}
