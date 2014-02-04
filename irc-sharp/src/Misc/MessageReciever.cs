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

using System;

namespace ircsharp
{
	public class MessageReciever : IRCBase
	{
		private string strNetworkIdentifier;
		private bool blIsChannel;
		
		/// <summary>
		/// Occurs when a user sends a message (/MSG) directly or to a channel.
		/// </summary>
		/// <seealso cref="MessageEventArgs"/>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event MessageEventHandler RecievedMessage;

		/// <summary>
		/// Occurs when a user sends a notice (/NOTICE) directly or to a channel.
		/// </summary>
		/// <seealso cref="MessageEventArgs"/>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event MessageEventHandler RecievedNotice;

		/// <summary>
		/// Occurs when a user sends a CtCp request.
		/// </summary>
		/// <seealso cref="CtCpMessageEventArgs"/>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event CtCpMessageEventHandler RecievedCtCpMessage;

		internal MessageReciever(ServerConnection creatorsCurrentConnection, string NetworkIdentifier, bool isChannel) : base(creatorsCurrentConnection)
		{
			strNetworkIdentifier = NetworkIdentifier;
			blIsChannel = isChannel;
		}
		
		internal MessageReciever(ServerConnection creatorsCurrentConnection, string NetworkIdentifier) : this(creatorsCurrentConnection, NetworkIdentifier, false)
		{
		}
		
		public string NetworkIdentifier
		{
			get { return strNetworkIdentifier; }
			internal set { strNetworkIdentifier = value; }
		}
		
		public bool IsChannel
		{
			get { return blIsChannel; }
		}

		/// <summary>
		/// CtCp pings the user.
		/// </summary>
		public void CtCpPing()
		{
			double milliseconds = ((TimeSpan) DateTime.Now.Subtract(new DateTime(2000, 1, 1))).TotalMilliseconds;
			base.CurrentConnection.SendData("PRIVMSG " + strNetworkIdentifier + " :\x01PING " + milliseconds.ToString() + "\x01");
		}

		/// <summary>
		/// CtCp versions the user.
		/// </summary>
		public void CtCpVersion()
		{
			base.CurrentConnection.SendData("PRIVMSG " + strNetworkIdentifier + " :\x01VERSION\x01");
		}
		
		/// <summary>
		/// CtCp times the user.
		/// </summary>
		public void CtCpTime()
		{
			base.CurrentConnection.SendData("PRIVMSG " + strNetworkIdentifier + " :\x01TIME\x01");
		}

		/// <summary>
		/// CtCps fingers the user.
		/// </summary>
		public void CtCpFinger()
		{
			base.CurrentConnection.SendData("PRIVMSG " + strNetworkIdentifier + " :\x01" + "FINGER\x01");
		}

		/// <summary>
		/// CtCps the user with an unsupported command.
		/// </summary>
		/// <param name="Command">The command the send the user.</param>
		public void CtCp(string Command)
		{
			CtCp(Command, null);
		}

		/// <summary>
		/// CtCps the user with an unsupported command, and parameters.
		/// </summary>
		/// <param name="Command">The command to send the user.</param>
		/// <param name="Parameters">The parameters to the command to give the user.</param>
		public void CtCp(string Command, string Parameters)
		{
			if (Parameters!=null&&Parameters!="")
				Parameters = " " + Parameters;
			base.CurrentConnection.SendData("PRIVMSG " + strNetworkIdentifier + " :\x01" + Command + Parameters + "\x01");
		}

		/// <summary>
		/// CtCp replys the user with a response to a command the user has sent.
		/// </summary>
		/// <param name="Command">The command to reply.</param>
		public void CtCpReply(string Command)
		{
			CtCpReply(Command, null);
		}

		/// <summary>
		/// CtCp replys the user with a response to a command the user has sent, and parameters to the command.
		/// </summary>
		/// <param name="Command">The command to reply.</param>
		/// <param name="Parameters">The parameters to the command you are replying.</param>
		public void CtCpReply(string Command, string Parameters)
		{
			if (Parameters!=null&&Parameters!="")
				Parameters = " " + Parameters;
			base.CurrentConnection.SendData("NOTICE " + strNetworkIdentifier + " :\01" + Command + Parameters + "\x01");
		}
		/// <summary>
		/// Sends a message to a users nick name or a channel.
		/// </summary>
		/// <param name="Message">The message to send.</param>
		public void SendMessage(string Message)
		{
			SendMessage(Message, false);
		}

		/// <summary>
		/// Sends an action message to a users nick name or a channel.
		/// </summary>
		/// <param name="Message">The message to send.</param>
		/// <param name="Action">If true sends a action message. If false sends a normal message.</param>
		public void SendMessage(string Message, bool Action)
		{
			if (Action)
				base.CurrentConnection.SendData("PRIVMSG " + strNetworkIdentifier + " :\x01" + "ACTION " + Message + "\x01");
			else
				base.CurrentConnection.SendData("PRIVMSG " + strNetworkIdentifier + " :" + Message);
		}

		/// <summary>
		/// Sends a notice to a users nick name or channel.
		/// </summary>
		/// <param name="Message">The message to notice.</param>
		public void Notice(string Message)
		{
			base.CurrentConnection.SendData("NOTICE " + strNetworkIdentifier + " :" + Message);
		}
		
		internal void FireRecievedMessage(User user, string message, bool isAction)
		{
			if (this.RecievedMessage != null)
				this.RecievedMessage(this, new MessageEventArgs(user, message, isAction));
		}
		
		internal void FireRecievedNotice(User user, string message)
		{
			if (this.RecievedNotice != null)
				this.RecievedNotice(this, new MessageEventArgs(user, message));
		}
		
		internal void FireRecievedCtCpMessage(User user, string command, string parameters)
		{
			if (this.RecievedCtCpMessage != null)
				this.RecievedCtCpMessage(this, new CtCpMessageEventArgs(user, command, parameters));
		}
	}
}
