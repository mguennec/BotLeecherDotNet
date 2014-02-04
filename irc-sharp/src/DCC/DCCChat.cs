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

namespace ircsharp
{
	public delegate void DCCChatEventHandler(DCCChat sender, EventArgs e);
	public delegate void DCCChatMessageEventHandler(DCCChat sender, DCCChatMessageEventArgs e);

	public class DCCChat : DCCBase
	{
		public event DCCChatEventHandler ConversationEtablished;
		public event DCCChatEventHandler ConversationTerminated;
		public event DCCChatMessageEventHandler Message;

		internal DCCChat(ServerConnection creatorsServerConnection, long RemoteHost, int Port, string Nickname, DCCChatContainer parent):base(creatorsServerConnection, RemoteHost, Port, Nickname, parent)
		{
		}

		internal DCCChat(ServerConnection creatorsServerConnection, string Nickname, int Port, DCCChatContainer parent):base(creatorsServerConnection, Nickname, Port, parent)
		{
		}

		/// <summary>
		/// Accepts the incoming DCC chat.
		/// </summary>
		public void Accept()
		{
			base.EtablishConnection();
		}

		protected override void Connected()
		{
			if (ConversationEtablished!=null)
				ConversationEtablished(this, new EventArgs());
		}

		protected override void Disconnected()
		{
			if (ConversationTerminated!=null)
				ConversationTerminated(this, new EventArgs());
		}

		protected override void OnData(byte[] buffer, int Length)
		{
			if (Message!=null)
				Message(this, new DCCChatMessageEventArgs(System.Text.Encoding.Default.GetString(buffer, 0, Length).Replace("\r", "").Replace("\n", "")));
		}

		/// <summary>
		/// Sends a message to the remote user.
		/// </summary>
		/// <param name="Message">The message to send.</param>
		public void SendMessage(string Message)
		{
			SendData(Message + Environment.NewLine);
		}

		/// <summary>
		/// Ends the DCC chat session.
		/// </summary>
		public void EndConversation()
		{
			CloseSocket();
		}
	}
}
