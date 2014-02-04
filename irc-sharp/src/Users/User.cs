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
	public class User : MessageReciever
	{
		private string strNick;
		private string strIdentity;
		private string strHost;
		private string strRealName;
		private string strUsingServer;

        private bool blHoldBackInfoUpdatedEvent;
        private bool blInfoWasUpdated;
		
		/// <summary>
		/// Occures when a user send a request for a DCC chat.
		/// </summary>
		/// <seealso cref="DCCChatRequestEventArgs"/>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event DCCChatRequestEventHandler DCCChatRequested;

		/// <summary>
		/// Occures when a user send a file.
		/// </summary>
		/// <seealso cref="DCCTransferRequestEventArgs"/>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event DCCTransferRequestEventHandler DCCTransferRequested;

		/// <summary>
		/// Occurs when a user replyes a CtCp message.
		/// </summary>
		/// <seealso cref="CtCpCommandReplyEventArgs"/>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event CtCpCommandReplyEventHandler CtCpReplied;

		/// <summary>
		/// Occurs when a user replyes a CtCp ping request.
		/// </summary>
		/// <seealso cref="CtCpPingReplyEventArgs"/>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event CtCpPingReplyEventHandler CtCpPingReplied;

		/// <summary>
		/// Occurs when a user replies a CtCp version request.
		/// </summary>
		/// <seealso cref="CtCpReplyEventArgs"/>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event CtCpReplyEventHandler CtCpVersionReplied;

		/// <summary>
		/// Occurs when a user replyes a CtCp finger request.
		/// </summary>
		/// <seealso cref="CtCpReplyEventArgs"/>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event CtCpReplyEventHandler CtCpFingerReplied;

		/// <summary>
		/// Occurs when a user replyes a CtCp time request.
		/// </summary>
		/// <seealso cref="CtCpReplyEventArgs"/>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event CtCpReplyEventHandler CtCpTimeReplied;

		/// <summary>
		/// Occurs when a user disconnects from the IRC server.
		/// </summary>
		/// <seealso cref="QuitEventArgs"/>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event QuitEventHandler Quitted;

		/// <summary>
		/// Occurs when a user changes his nick, but before the nick is changed in the Channels array.
		/// </summary>
		/// <seealso cref="NickChangeEventArgs"/>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event BeforeNickChangeEventHandler BeforeNickChange;

		/// <summary>
		/// Occurs when a user changes his nick.
		/// </summary>
		/// <seealso cref="NickChangeEventArgs"/>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event NickChangeEventHandler NickChange;

		public event UserInfoEventHandler InfoUpdated;
		
		internal User(ServerConnection creatorsServerConnection, string Nick, string Identity, string Host, string RealName) : base(creatorsServerConnection, Nick)
		{
			strNick = Nick;
			strIdentity = Identity;
			strHost = Host;
			strRealName = RealName;

            blHoldBackInfoUpdatedEvent = false;
            blInfoWasUpdated = false;
		}
		
		internal User(ServerConnection creatorsServerConnection, string Nick, string Identity, string Host, bool askWho) : this(creatorsServerConnection, Nick, Identity, Host, null)
		{
			if (askWho && Nick.IndexOf(".") == - 1) // "." Indicates server. Don't ask who.
				base.CurrentConnection.SendData("WHO {0}", strNick);
		}
		
		internal User(ServerConnection creatorsServerConnection, string Nick, bool askWho) : this(creatorsServerConnection, Nick, null, null, askWho)
		{
		}
		
		internal void SetAllInfo(UserInfo userInfo, string RealName, string UsingServer)
		{
			strNick = userInfo.Nick;
			strIdentity = userInfo.Identity;
			strHost = userInfo.Host;
			strRealName = RealName;
			strUsingServer = UsingServer;
			if (InfoUpdated != null)
				InfoUpdated(this, new EventArgs());
		}

        internal bool HoldBackInfoUpdatedEvent
        {
            set
            {
                blHoldBackInfoUpdatedEvent = value;
                if (blHoldBackInfoUpdatedEvent == false && blInfoWasUpdated)
                    FireInfoUpdated();
            }
        }

		public string Nick
		{
			get { return strNick; }
			internal set
			{
				strNick = value;
				base.NetworkIdentifier = strNick;
				if (InfoUpdated != null)
					InfoUpdated(this, new EventArgs());
			}
		}
		
		public string Identity
		{
			get { return strIdentity; }
			internal set
			{
                if (strIdentity != value)
                {
                    strIdentity = value;
                    FireInfoUpdated();
                }
			}
		}
		
		public string Host
		{
			get { return strHost; }
			internal set
			{
                if (strHost != value)
                {
                    strHost = value;
                    FireInfoUpdated();
                }
			}
		}
		
		public string RealName
		{
			get { return strRealName; }
			internal set
			{
                if (strRealName != value)
                {
                    strRealName = value;
                    FireInfoUpdated();
                }
			}
		}
		
		public string UsingServer
		{
			get { return strUsingServer; }
		}
		
		/// <summary>
		/// Invite the user to a specified channel.
		/// </summary>
		/// <param name="ChannelName">Channel name to invite user to.</param>
		public void Invite(string ChannelName)
		{
			base.CurrentConnection.SendData("INVITE " + strNick + " " + ChannelName);
		}

        internal void FireInfoUpdated()
        {
            if (!blHoldBackInfoUpdatedEvent)
            {
                if (InfoUpdated != null)
                    InfoUpdated(this, new EventArgs());
                blInfoWasUpdated = false;
            }
            else
                blInfoWasUpdated = true;
        }
		
		internal void FireDCCChatRequest(DCCChat newChat)
		{
			if (this.DCCChatRequested != null)
				this.DCCChatRequested(this, new DCCChatRequestEventArgs(newChat));
		}
		
		internal void FireDCCTransferRequest(DCCTransfer newTransfer)
		{
			if (this.DCCTransferRequested != null)
				this.DCCTransferRequested(this, new DCCTransferRequestEventArgs(newTransfer));
		}
		
		internal void FireCtCpReplied(string command, string reply)
		{
			if (this.CtCpReplied != null)
				this.CtCpReplied(this, new CtCpCommandReplyEventArgs(command, reply));
		}
		
		internal void FireCtCpPingReplied(TimeSpan time)
		{
			if (this.CtCpPingReplied != null)
				this.CtCpPingReplied(this, new CtCpPingReplyEventArgs(time));
		}
		
		internal void FireCtCpVersionReplied(string reply)
		{
			if (this.CtCpVersionReplied != null)
				this.CtCpVersionReplied(this, new CtCpReplyEventArgs(reply));
		}
		
		internal void FireCtCpFingerReplied(string reply)
		{
			if (this.CtCpFingerReplied != null)
				this.CtCpFingerReplied(this, new CtCpReplyEventArgs(reply));
		}
		
		internal void FireCtCpTimeReplied(string reply)
		{
			if (this.CtCpTimeReplied != null)
				this.CtCpTimeReplied(this, new CtCpReplyEventArgs(reply));
		}
		
		internal void FireQuitted(string reason)
		{
			if (this.Quitted != null)
				this.Quitted(this, new QuitEventArgs(reason));
		}
		
		internal void FireBeforeNickChange(string newNick)
		{
			if (this.BeforeNickChange != null)
				this.BeforeNickChange(this, new BeforeNickChangeEventArgs(newNick));
		}
		
		internal void FireNickChange(string oldNick)
		{
			if (this.NickChange != null)
				this.NickChange(this, new NickChangeEventArgs(oldNick));
		}
	}
}
