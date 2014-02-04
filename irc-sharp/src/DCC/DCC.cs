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
using System.Net;
using System.Net.Sockets;
using System.IO;
using ircsharp;

namespace ircsharp
{
	/// <summary>
	/// This class is used to control DCC. This class can only be instanced by Connection.
	/// </summary>
	public class DCC : IRCBase
	{
		private DCCChatContainer chat;
		private DCCTransferContainer transfer;
		private Connection parent;

		internal DCC(ServerConnection creatorsServerConnection, Connection Parent) : base(creatorsServerConnection)
		{
			chat = new DCCChatContainer(creatorsServerConnection);
			transfer = new DCCTransferContainer(creatorsServerConnection);
			parent = Parent;
		}

		/// <summary>
		/// Request a chat to a user.
		/// </summary>
		/// <param name="Nickname">The nick name of the user to chat request.</param>
		/// <returns>A DCC chat.</returns>
		public DCCChat RequestChat(string Nickname)
		{
			if (parent.Channels.IsOnChannel(Nickname))
				throw(new Exception("Cannot DCC request channels."));

			int Port = FreePort();
			DCCChat newChat = new DCCChat(base.CurrentConnection, Nickname, Port, chat);
			chat.AddDcc(newChat);
			base.CurrentConnection.SendData(String.Format("NOTICE {0} :DCC Chat ({1}){2}PRIVMSG {0} :\x01{5} CHAT chat {3} {4}\x01{2}",
                Nickname, 
                parent.LocalIP.ToString(), 
                Environment.NewLine, 
                IRCHost(parent.LocalIP.ToString()), 
                Port.ToString(), 
                "DCC"));
			return newChat;
		}

		/// <summary>
		/// Send a request to send a file to a user.
		/// </summary>
		/// <param name="Nickname">The nick name of the user to receive the file.</param>
		/// <param name="Filename">The filename of the file to send.</param>
		/// <returns>A DCC transfer.</returns>
		public DCCTransfer SendFile(string Nickname, string Filename)
		{
			if (parent.Channels.IsOnChannel(Nickname))
				throw(new Exception("Cannot DCC request channels."));

			if (File.Exists(Filename))
			{
				int Port = FreePort();
				string strRemote = Filename;
				if (Filename.IndexOf("\\")>-1)
					strRemote = Filename.Substring(Filename.LastIndexOf("\\") + 1, Filename.Length - (Filename.LastIndexOf("\\") + 1));
				strRemote.Replace(" ", "_");
				FileInfo fileInfo = new FileInfo(Filename);
				DCCTransfer newTransfer = new DCCTransfer(base.CurrentConnection, Nickname, Port, Filename, strRemote, transfer);
				transfer.AddDcc(newTransfer);
				base.CurrentConnection.SendData("NOTICE " + Nickname + " :DCC Send " + strRemote + " (" + parent.LocalIP.ToString() + ")");
				base.CurrentConnection.SendData("PRIVMSG " + Nickname + " :\x01" + "DCC SEND " + strRemote + " " + IRCHost(parent.LocalIP.ToString()) + " " + Port.ToString() + " " + fileInfo.Length.ToString() + "\x01");

				return newTransfer;
			}
			else
				throw(new FileNotFoundException("The file was not found.", Filename));
		}

		private string IRCHost(string DotIP)
		{
			string[] IPs = DotIP.Split(new char[] {'.'});
			long[] IPl = {long.Parse(IPs[0]) * (256*256*256), long.Parse(IPs[1]) * (256*256), long.Parse(IPs[2]) * 256, long.Parse(IPs[3])};
			long IP = IPl[0] + IPl[1] + IPl[2] + IPl[3];
			return IP.ToString();
		}

		private int FreePort()
		{
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			
			for (int x=1024;x<1048;x++)
			{
				try 
				{ 
					socket.Bind(new IPEndPoint(IPAddress.Any, x));
					socket.Listen(0);
					socket.Close();
					return x;
				}
				catch {}
			}

			return 1024;
		}

		/// <summary>
		/// Gets an array of currently open DCC chats.
		/// </summary>
		public DCCChatContainer Chats
		{
			get { return chat; }
		}

		/// <summary>
		/// Gets an array of the currently transfering files.
		/// </summary>
		public DCCTransferContainer Transfer
		{
			get { return transfer; }
		}
	}
}
