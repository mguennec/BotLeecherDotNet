#region License Agreement - READ THIS FIRST!!!
/*
 **********************************************************************************
 * Copyright (c) 2008, Kristian Trenskow                                          *
 * All rights reserved.                                                           *
 **********************************************************************************
 * This code is subject to terms of the BSD License. A copy of this license is    *
 * included with this software distribution in the file COPYING. If you do not    *
 * have a copy of, you can contain a copy by visiting this URL:                   *
 *                                                                                *
 * http://www.opensource.org/licenses/bsd-license.php                             *
 *                                                                                *
 * Replace <OWNER>, <ORGANISATION> and <YEAR> with the info in the above          *
 * copyright notice.                                                              *
 *                                                                                *
 **********************************************************************************
 */
#endregion
using System;
using System.IO;

namespace ircsharp
{
	/// <summary>
	/// An enum with info on what to do when accepting a file that already exist.
	/// </summary>
	public enum DCCTransferFileExist
	{
        /// <summary>
        /// Overwrite local file.
        /// </summary>
		Overwrite = 0,
        /// <summary>
        /// Resume file from local filesize.
        /// </summary>
		Resume,
        /// <summary>
        /// Ignore file transfer.
        /// </summary>
		Ignore
	}

	internal enum DCCTransferDirection
	{
		Send = 0,
		Receive
	}

    /// <summary>
    /// Used with the TransferBegun, TransferFailed and TransferComplete events.
    /// </summary>
	public delegate void DCCTransferEventHandler(DCCTransfer sender, EventArgs e);
    /// <summary>
    /// Used with the TransferProgress event.
    /// </summary>
	public delegate void DCCTransferProgressEventHandler(DCCTransfer sender, DCCTransferProgressEventArgs e);

    /// <summary>
    /// Class represents a DCC transfer.
    /// </summary>
	public class DCCTransfer : DCCBase
	{
		private long lngSize = 0;
        private Stream localStream = null;
		private string strLocalFile = "";
		private string strRemoteFile = "";
		private long lngBytesTransfered = 0;

        private DCCTransferDirection direction;

        public Stream TransferStream { get { return localStream; } }

		public event DCCTransferEventHandler TransferBegun;
		public event DCCTransferEventHandler TransferFailed;
		public event DCCTransferProgressEventHandler TransferProgress;
        public event DCCTransferEventHandler TransferComplete;


		internal DCCTransfer(ServerConnection creatorsServerConnection, long RemoteHost, int Port, string Nickname, string Filename, long Size, DCCTransferContainer parent):base(creatorsServerConnection, RemoteHost, Port, Nickname, parent)
		{
			lngSize = Size;
			strRemoteFile = Filename;
			direction = DCCTransferDirection.Receive;
		}

		internal DCCTransfer(ServerConnection creatorsServerConnection, string Nickname, int Port, string Filename, string RemoteFilename, DCCTransferContainer parent):base(creatorsServerConnection, Nickname, Port, parent)
		{
			direction = DCCTransferDirection.Send;
			strLocalFile = Filename;
			strRemoteFile = RemoteFilename;
			FileInfo fileInfo = new FileInfo(Filename);
			lngSize = fileInfo.Length;
		}

		/// <summary>
		/// Gets the filename of the local file.
		/// </summary>
		/// <remarks>If a file is recieved, this is the file name where the incoming file is saved. If file is sent, this is the file name of the file being sent.</remarks>
		public string LocalFile
		{
			get { return strLocalFile; }
		}

		/// <summary>
		/// Gets the filename of the remote file.
		/// </summary>
		/// <remarks>This is the filename used between the client. It's equal to LocalFile just that it's without path, and space is replaced by _.</remarks>
		public string RemoteFile
		{
			get { return strRemoteFile; }
		}

		/// <summary>
		/// Gets the number of bytes transfered.
		/// </summary>
		public long BytesTransfered
		{
			get { return lngBytesTransfered; }
		}

		/// <summary>
		/// Gets the total number of bytes to be transfered.
		/// </summary>
		public long BytesTotal
		{
			get { return lngSize; }
		}

        /// <summary>
        /// Accept the incoming DCC file request.
        /// </summary>
        /// <param name="Filename">The path and file name where the incoming file is to be saved.</param>
        public void Accept(string Filename)
        {
            Accept(Filename, DCCTransferFileExist.Overwrite);
        }

        /// <summary>
        /// Accept the incoming DCC file request.
        /// </summary>
        /// <param name="Filename">The path and file name where the incoming file is to be saved.</param>
        /// <param name="Action">What to do in case the file already exist.</param>
        public void Accept(string Filename, DCCTransferFileExist Action = DCCTransferFileExist.Overwrite)
        {
            strLocalFile = Filename;
            if (File.Exists(Filename))
            {
                switch (Action)
                {
                    case DCCTransferFileExist.Resume:
                        FileInfo fileInfo = new FileInfo(Filename);
                        base.CurrentConnection.SendData("PRIVMSG " + base.Nick + " :\x01" + "DCC RESUME " + strRemoteFile + " " + base.Identifier.ToString() + " " + fileInfo.Length.ToString() + "\x01");
                        return;
                    case DCCTransferFileExist.Ignore:
                        return;
                    case DCCTransferFileExist.Overwrite:
                        File.Delete(Filename);
                        break;
                }
            }

            base.EtablishConnection();
        }

        /// <summary>
        /// Accept the incoming DCC file request.
        /// </summary>
        /// <param name="Filename">The path and file name where the incoming file is to be saved.</param>
        /// <param name="Action">What to do in case the file already exist.</param>
        public void Accept(Stream stream, DCCTransferFileExist Action = DCCTransferFileExist.Overwrite)
        {
            this.localStream = stream;
            if (stream != null)
            {
                switch (Action)
                {
                    case DCCTransferFileExist.Ignore:
                        return;
                }
            }

            base.EtablishConnection();
        }

		internal void ResumeAccepted(long lngPosition)
		{
			lngBytesTransfered = lngPosition;
			base.EtablishConnection();
		}

		internal void ResumeRequested(long lngPosition)
		{
			if (lngPosition<=lngSize)
			{
				lngBytesTransfered = lngPosition;
				base.CurrentConnection.SendData("PRIVMSG " + base.strNickname + " :\x01" + "DCC ACCEPT " + strRemoteFile + " " + base.Identifier.ToString() + " " + lngPosition.ToString() + "\x01");
			}
		}

		protected override void Connected()
		{
			if (TransferBegun!=null)
				TransferBegun(this, new EventArgs());

			if (direction==DCCTransferDirection.Send)
				SendNextChunk();
		}

		protected override void Disconnected()
		{
			if (lngBytesTransfered!=lngSize&&TransferFailed!=null)
				TransferFailed(this, new EventArgs());
		}

		protected override void OnData(byte[] buffer, int Length)
		{
			if (direction==DCCTransferDirection.Send)
			{
				long lngCBR = OctetToLong(buffer, Length);
				if (lngCBR!=lngBytesTransfered)
				{
					if (TransferFailed!=null)
						TransferFailed(this, new EventArgs());
					CloseSocket();
				}
				else
				{
					if (TransferProgress!=null)
						TransferProgress(this, new DCCTransferProgressEventArgs(lngBytesTransfered, lngSize));

					if (lngBytesTransfered==lngSize&&TransferComplete!=null)
					{
						TransferComplete(this, new EventArgs());
                        localStream = null;
						CloseSocket();
					}
					else
						SendNextChunk();
				}
			}
			else
			{
                Stream transferStream;
                if (localStream != null)
                {
                    transferStream = localStream;
                }
                else
                {
                    FileStream file = File.OpenWrite(strLocalFile);
                    file.Seek(lngBytesTransfered, SeekOrigin.Begin);
                    transferStream = file;
                }
                transferStream.Write(buffer, 0, Length);
				lngBytesTransfered += Length;
                if (localStream == null)
                {
                    transferStream.Close();
                }
				SendData(LongToOctet(lngBytesTransfered), lngBytesTransfered==lngSize);
				if (TransferProgress!=null)
					TransferProgress(this, new DCCTransferProgressEventArgs(lngBytesTransfered, lngSize));
                if (lngBytesTransfered == lngSize && TransferComplete != null)
                {
                    TransferComplete(this, new EventArgs());
                }
			}
		}

		/// <summary>
		/// Cancels the transfer.
		/// </summary>
		public void CancelTransfer()
		{
			CloseSocket();
		}

		private void SendNextChunk()
		{
			int intLength = 1024;
			
			FileStream file = File.OpenRead(strLocalFile);
			
			if (file.Length-lngBytesTransfered<intLength)
				intLength = Convert.ToInt32(file.Length - lngBytesTransfered);

			file.Seek(lngBytesTransfered, SeekOrigin.Begin);
			byte[] toSend = new byte[intLength];
			int intRead = file.Read(toSend, 0, intLength);
			file.Close();
			lngBytesTransfered += intRead;
			SendData(toSend);
		}

		private long OctetToLong(byte[] octets, int Length)
		{
			long ret = 0;
			long currentPos = 16777216;

			for (int x=0;x<4;x++)
			{
				ret += (long) (octets[x] * currentPos);
				currentPos /= 256;
			}

			return ret;
		}

		private byte[] LongToOctet(long Value)
		{
			byte[] ret = new byte[4];
			long currentByte = 16777216;

			for (int x=0;x<4;x++)
			{
				ret[x] = Convert.ToByte(Math.Floor((double)Value / (double)currentByte));
				Value = Value - (ret[x] * currentByte);
				currentByte /= 256;
			}

			return ret;
		}
	}
}
