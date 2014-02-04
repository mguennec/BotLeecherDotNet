using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher.Tools
{
    public class ReceiveFileTransfer : FileTransfer
    {
        private string p1;
        private ircsharp.DCCTransfer dCCTransfer;
        private int p2;

        public ReceiveFileTransfer() { }        

        protected override void TransferFile()
        {
            Stream s = new NetworkStream(Socket.Result);
            StreamWriter socketOutput = new StreamWriter(s);
            FileStream fileOutput = new FileStream(File, FileMode.OpenOrCreate, FileAccess.Write);
            try
            {
                fileOutput.Position = StartPosition;
                //Recieve file
                byte[] inBuffer = new byte[1024];
                byte[] outBuffer = new byte[4];
                int bytesRead = 0;
                while ((bytesRead = s.Read(inBuffer, 0, inBuffer.Length)) != -1)
                {
                    fileOutput.Write(inBuffer, 0, bytesRead);
                    BytesTransfered += bytesRead;
                    //Send back an acknowledgement of how many bytes we have got so far.
                    //Convert bytesTransfered to an "unsigned, 4 byte integer in network byte order", per DCC specification
                    outBuffer[0] = (byte)((BytesTransfered >> 24) & 0xff);
                    outBuffer[1] = (byte)((BytesTransfered >> 16) & 0xff);
                    outBuffer[2] = (byte)((BytesTransfered >> 8) & 0xff);
                    outBuffer[3] = (byte)(BytesTransfered & 0xff);
                    socketOutput.Write(outBuffer);
                    OnAfterSend();
                }
            }
            finally
            {
                fileOutput.Close();
                socketOutput.Close();
                s.Close();
            }
        }

        protected override void TransferString(StringBuilder sb)
        {
            Stream s = new NetworkStream(Socket.Result);
            StreamReader reader = new StreamReader(s);
            StreamWriter socketOutput = new StreamWriter(s);
            StringWriter writer = new StringWriter(sb);
            try
            {
                //Recieve file
                char[] inBuffer = new char[1024];
                byte[] outBuffer = new byte[4];
                int bytesRead = 0;
                while ((bytesRead = reader.Read(inBuffer, 0, inBuffer.Length)) != -1)
                {
                    writer.Write(inBuffer, 0, bytesRead);
                    BytesTransfered += bytesRead;
                    //Send back an acknowledgement of how many bytes we have got so far.
                    //Convert bytesTransfered to an "unsigned, 4 byte integer in network byte order", per DCC specification
                    outBuffer[0] = (byte)((BytesTransfered >> 24) & 0xff);
                    outBuffer[1] = (byte)((BytesTransfered >> 16) & 0xff);
                    outBuffer[2] = (byte)((BytesTransfered >> 8) & 0xff);
                    outBuffer[3] = (byte)(BytesTransfered & 0xff);
                    socketOutput.Write(outBuffer);
                    OnAfterSend();
                }
            }
            finally
            {
                socketOutput.Close();
                writer.Close();
                reader.Close();
                s.Close();
            }
        }
    }
}
