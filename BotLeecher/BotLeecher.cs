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
using System.Net.NetworkInformation;
using ircsharp;

namespace BotLeecher
{
    public class BotLeecher
    {
        public static readonly ILog LOGGER = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Task Task;
        public readonly LeecherQueue Queue;
        public Settings Settings { get; set; }
        public PackListReader PackListReader { get; set; }
        public User BotUser { get; set; }
        public string Description { get; set; }
        public IrcConnection Connection { get; set; }
        public bool Leeching { get; set; }
        public bool Downloading { get; set; }
        public bool ListRequested { get; set; }
        public int Counter { get; set; }
        public DCCTransfer CurrentTransfer { get; set; }
        public long FileSize { get; set; }
        public string ListFile { get; set; }
        public IList<BotListener> Listeners { get; set; }
        public DateTime StartTime { get; set; }
        public PackList PackList { get; set; }
        public String FileName { get; set; }
        

        public BotLeecher(User user, IrcConnection connection, Settings settings, PackListReader packListReader)
        {
        
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
            this.Queue = QueueManager.GetQueue(this);
            user.DCCTransferRequested += OnDccTransfer;
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

        private void OnDccTransfer(User sender, DCCTransferRequestEventArgs e)
        {
            FileSize = e.NewTransfer.BytesTotal;
            FileName = e.NewTransfer.RemoteFile;

            CurrentTransfer = e.NewTransfer;
            CurrentTransfer.TransferProgress += OnTransferProgress;
            CurrentTransfer.TransferComplete += OnTransferComplete;
            CurrentTransfer.TransferBegun += OnTransferBegun;
            CurrentTransfer.TransferFailed += OnTransferFailed;
            Queue.OnIncomingFileTransfer();
        }

        private void OnTransferFailed(DCCTransfer sender, EventArgs e)
        {
            var stream = sender.TransferStream;
            if (stream != null)
            {
                stream.Close();
            }
            else
            {
                ChangeState(CurrentTransfer.RemoteFile, new PackStatus(PackStatus.Status.AVAILABLE));
                File.Delete(CurrentTransfer.LocalFile);
            }
            FireFailure();
            ResetTransferState();
            FireStatusEvent();
        }

        private void FireFailure()
        {
            foreach (BotListener listener in Listeners)
            {
                listener.Failure(BotUser, FileName);
            }
        }

        private void OnTransferBegun(DCCTransfer sender, EventArgs e)
        {
            FireBegin();
        }

        private void FireBegin()
        {
            foreach (BotListener listener in Listeners)
            {
                listener.Beginning(BotUser, FileName);
            }
        }

        private void OnTransferComplete(DCCTransfer sender, EventArgs e)
        {
            var stream = sender.TransferStream;
            if (stream != null)
            {
                stream.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(stream);
                var sb = new StringBuilder(reader.ReadToEnd());
                LOGGER.Info("LIST:\t List received for " + BotUser.Nick);
                ListRequested = false;
                PackList = PackListReader.ReadPacks(sb);
                reader.Close();
                stream.Close();
                foreach (String message in PackList.Messages)
                {
                    Description += message + "\n";
                }
                FireListEvent();
            }
            else
            {
                DownloadFinished(CurrentTransfer.RemoteFile);
            }
            FireComplete();
            ResetTransferState();
            FireStatusEvent();
        }

        private void ResetTransferState()
        {

            Downloading = false;
            CurrentTransfer = null;
            FileName = null;
        }

        private void FireComplete()
        {
            foreach (BotListener listener in Listeners)
            {
                listener.Complete(BotUser, FileName);
            }
        }

        private void OnTransferProgress(DCCTransfer sender, DCCTransferProgressEventArgs e)
        {
            FireStatusEvent();
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
                long diff = (DateTime.Now.Ticks - StartTime.Ticks) / 1000;
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
            private BlockingCollection<int> InternalQueue = new BlockingCollection<int>();
            public BotLeecher BotLeecher;

            public LeecherQueue(BotLeecher botLeecher) {
                this.BotLeecher = botLeecher;
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
                    int nr = InternalQueue.Take();
                    AskPack(nr);
                }
            }

            private void AskPack(int nr)
            {
                if (Canceled) {
                    Canceled = false;
                    BotLeecher.ChangeState(nr, new PackStatus(PackStatus.Status.AVAILABLE));
                } else {
                    if (1 == nr) {
                        BotLeecher.ListRequested = true;
                    }
                    BotLeecher.BotUser.SendMessage("XDCC SEND " + nr);
                }
            }

            /**
             * @param transfer
             */
            public void OnIncomingFileTransfer() {
                if (BotLeecher.ListRequested) {
                    var stream = new MemoryStream();
                    BotLeecher.CurrentTransfer.Accept(stream);
                } else {
                    BotLeecher.ChangeState(BotLeecher.FileName, new PackStatus(PackStatus.Status.DOWNLOADING));
                    LOGGER.Info("INCOMING:\t" + BotLeecher.CurrentTransfer.RemoteFile + " " +
                            BotLeecher.FileSize + " bytes");

                    //if file exists cut one 8bytes off to make transfer go on
                    if (File.Exists(BotLeecher.CurrentTransfer.RemoteFile) && (BotLeecher.FileSize == new FileInfo(BotLeecher.CurrentTransfer.RemoteFile).Length))
                    {
                        LOGGER.Info("EXISTS:\t try to close connection");

                        //FileImageInputStream fis = new FileInputStream
                    } else {
                        LOGGER.Info("SAVING TO:\t" + BotLeecher.CurrentTransfer.RemoteFile);
                        try {
                            BotLeecher.Downloading = true;
                            BotLeecher.StartTime = DateTime.Now;
                            BotLeecher.CurrentTransfer.Accept(GetFileName(BotLeecher.CurrentTransfer.RemoteFile));
                        } catch (IOException e) {
                            BotLeecher.ChangeState(BotLeecher.CurrentTransfer.RemoteFile, new PackStatus(PackStatus.Status.AVAILABLE));
                            File.Delete(BotLeecher.CurrentTransfer.RemoteFile);
                            LOGGER.Error(e.Message, e);
                        }
                    }
                }
            }

            private string GetFileName(string fileName)
            {
                var path = BotLeecher.Settings.Get(SettingProperty.PROP_SAVEFOLDER).GetFirstValue();
                return path + Path.DirectorySeparatorChar + fileName;
            }

            public void Cancel() {
                Canceled = true;
                IList<int> list = InternalQueue.ToList();
                ClearQueue();
                foreach (int id in list) {
                    BotLeecher.ChangeState(id, new PackStatus(PackStatus.Status.AVAILABLE));
                }
                BotLeecher.BotUser.SendMessage("XDCC CANCEL");
            }

            private void ClearQueue()
            {
                while (InternalQueue.Count > 0)
                {
                    int item;
                    InternalQueue.TryTake(out item);
                }
            }
            

        }
        public class QueueManager
        {
            private static IDictionary<string, LeecherQueue> queues = new Dictionary<string, LeecherQueue>();


            public static LeecherQueue GetQueue(BotLeecher leecher)
            {
                if (!queues.ContainsKey(leecher.BotUser.Nick))
                {
                    queues.Add(leecher.BotUser.Nick, new LeecherQueue(leecher));
                }
                var queue = queues[leecher.BotUser.Nick];
                queue.BotLeecher = leecher;
                return queue;
            }
        }
    }

}
