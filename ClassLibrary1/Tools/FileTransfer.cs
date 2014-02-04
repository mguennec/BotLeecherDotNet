using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher.Tools
{
    public abstract class FileTransfer
    {

        public enum DccState {
            INIT, RUNNING, DONE
        }

        public Task<Socket> Socket { get; protected set; }
        public string User { get; protected set; }
        public string File { get; protected set; }
        public long StartPosition { get; protected set; }
        public long BytesTransfered { get; protected set; }
        protected DccState State = DccState.INIT;
        protected readonly object StateLock = new object();

        public FileTransfer() { }

        public FileTransfer(Task<Socket> socket, string user, string file, long startPosition) {
            this.Socket = socket;
            this.User = user;
            this.File = file;
            this.StartPosition = startPosition;
        }
        
        /**
         * Transfer the file to the user
         * @throws IOException If an error occurred during transfer
         */
        public void Transfer() {
                //Prevent being called multiple times
                if (State != DccState.INIT)
                        lock(StateLock) {
                                if (State != DccState.INIT)
                                    throw new InvalidOperationException("Cannot receive file twice (Current state: " + State + ")");
                        }
                State = DccState.RUNNING;

                TransferFile();

                State = DccState.DONE;
        }

        public void TransferToString(StringBuilder sb)
        {
            //Prevent being called multiple times
            if (State != DccState.INIT)
                lock (StateLock)
                {
                    if (State != DccState.INIT)
                        throw new InvalidOperationException("Cannot receive file twice (Current state: " + State + ")");
                }
            State = DccState.RUNNING;

            TransferString(sb);

            State = DccState.DONE;
        }

        protected abstract void TransferFile();

        protected abstract void TransferString(StringBuilder sb);
        
        /**
         * Callback at end of read/write loop:
         * <p>
         * Receive: Socket read -> file write -> socket write (bytes transferred) -> callback -> repeat
         * <p>
         * Send: File read -> socket write -> socket read (bytes transferred) -> callback -> repeat
         */
        protected void OnAfterSend() {  
        }
        
        /**
         * Is the transfer finished?
         * @return True if its finished
         */
        public bool IsFinished() {
                return State == DccState.DONE; 
        }
    }
}
