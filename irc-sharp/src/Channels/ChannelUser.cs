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
    /// Describes the users status on channel.
    /// </summary>
    public enum ChannelUserStatus
    {
        UnknownStatus = 0,
        Voiced = 1,
        HalfOperator = 2,
        Operator = 4,
        NotAStatus = 8
    }

	/// <summary>
	/// Summary description for ChannelUser.
	/// </summary>
	public class ChannelUser : IRCBase
	{
		private string strChannelName = "";
		private User objUser;
		private bool isOperator = false;
		private bool isVoiced = false;
        private bool isHalfOperator = false;
		private object objTag = null;

		internal ChannelUser(ServerConnection creatorsCurrentConnection, User user, string strChannelName) : base(creatorsCurrentConnection)
		{
			objUser = user;
		}

		public User User
		{
			get { return objUser; }
		}
		
		/// <summary>
		/// Gets the users operator status.
		/// </summary>
		public bool IsOperator
		{
			get { return isOperator; }
			internal set { isOperator = value; }
		}

		/// <summary>
		/// Gets the users voiced status.
		/// </summary>
		public bool IsVoiced
		{
			get { return isVoiced; }
			internal set { isVoiced = value; }
		}

        /// <summary>
        /// Gets the users half operator status.
        /// </summary>
        /// <remarks>Hybrid IRCd only.</remarks>
        public bool IsHalfOperator
        {
            get { return isHalfOperator; }
			internal set { isHalfOperator = value; }
        }

		/// <summary>
		/// Kicks the user from the channel with a specific reason.
		/// </summary>
		/// <param name="Reason">The reason why the user is kicked.</param>
		public void Kick(string Reason)
		{
			base.CurrentConnection.SendData("KICK " + strChannelName + " " + objUser.Nick + " :" + Reason + "\r\n");
		}

		/// <summary>
		/// Kicks the user from the channel.
		/// </summary>
		public void Kick()
		{
			Kick("");
		}

		/// <summary>
		/// Bans the user from the channel.
		/// </summary>
		public void Ban()
		{
			string strHostmask = objUser.Nick + "!*@*";
			if (objUser.Identity != null && objUser.Identity != "")
				strHostmask = "*!" + objUser.Identity + "@" + objUser.Host;
			base.CurrentConnection.SendData("MODE " + strChannelName + " +b " + strHostmask + "\r\n");
		}
		
		public void KickBan(string Reason)
		{
			Kick(Reason);
			Ban();
		}
		
		public void KickBan()
		{
			KickBan("");
		}

		/// <summary>
		/// Object to attach the user.
		/// </summary>
		public object Tag
		{
			get { return objTag; }
			set { objTag = value; }
		}
    }
}
