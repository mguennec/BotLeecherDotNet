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
using ircsharp;

namespace ircsharp
{
	/// <summary>
	/// Summary description for ChannelList.
	/// </summary>
	public class ChannelList : IRCBase
	{
		private string strChannelName = "";
		private int intUserCount = 0;
		private string strTopic = "";

		internal ChannelList(ServerConnection creatorsCurrentConnection, string channelName, int userCount, string topic) : base(creatorsCurrentConnection)
		{
			strChannelName = channelName;
			intUserCount = userCount;
			strTopic = topic;
		}

		/// <summary>
		/// Gets the name of the channel.
		/// </summary>
		public string Name
		{
			get { return strChannelName; }
		}

		/// <summary>
		/// Gets the number of users on the channel.
		/// </summary>
		public int UserCount
		{
			get { return intUserCount; }
		}

		/// <summary>
		/// Gets the channel topic.
		/// </summary>
		public string Topic
		{
			get { return strTopic; }
		}

		/// <summary>
		/// Joins the channel.
		/// </summary>
		public void Join()
		{
			base.CurrentConnection.SendData("JOIN " + strChannelName + "\r\n");
		}

		/// <summary>
		/// Join the channel using a specific key.
		/// </summary>
		/// <param name="Key">The key to use to access the channel</param>
		public void Join(string Key)
		{
			base.CurrentConnection.SendData("JOIN " + strChannelName + " :" + Key);
		}
	}
}
