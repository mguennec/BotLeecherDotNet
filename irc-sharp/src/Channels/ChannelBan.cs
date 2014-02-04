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
	/// <summary>
	/// Summary description for IRCBan.
	/// </summary>
	public class ChannelBan : IRCBase
	{
		private string strChannelName = "";
		private string strHostmask = "";
		private string strSetBy = "";
		private DateTime dtSetTime;
		private object objTag = null;

		internal ChannelBan(ServerConnection creatorsServerConnection, string channelName, string hostMask, string setBy, DateTime setTime) : base(creatorsServerConnection)
		{
			strChannelName = channelName;
			strHostmask = hostMask;
			strSetBy = setBy;
			dtSetTime = setTime;
		}

		/// <summary>
		/// Gets the host mask of the ban.
		/// </summary>
		public string HostMask
		{
			get { return strHostmask; }
		}

		/// <summary>
		/// Gets the nick of the user who set the ban.
		/// </summary>
		public string SetBy
		{
			get { return strSetBy; }
		}

		/// <summary>
		/// Gets the time whereas the ban was set.
		/// </summary>
		public DateTime SetTime
		{
			get { return dtSetTime; }
		}

		/// <summary>
		/// Use this to attach an object to the ban.
		/// </summary>
		public object Tag
		{
			get { return objTag; }
			set { objTag = value; }
		}

		/// <summary>
		/// Remove this ban from the channel.
		/// </summary>
		/// <remarks>Required channel operator status to set.</remarks>
		public void Unban()
		{
			base.CurrentConnection.SendData("MODE " + strChannelName + " -b " + strHostmask + "\r\n");
		}
	}
}
