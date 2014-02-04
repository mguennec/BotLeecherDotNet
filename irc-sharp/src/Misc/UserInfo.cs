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
	internal class UserInfo
	{
		private string strNick = "";
		private string strIdentity = "";
		private string strHost = "";

		internal UserInfo(string Nick, string Identity, string Host)
		{
			strNick = Nick;
			strIdentity = Identity;
			strHost = Host;
		}

        /// <summary>
        /// Gets the nick name used on server.
        /// </summary>
		public string Nick
		{
			get { return strNick; }
			internal set { strNick = value; }
		}

        /// <summary>
        /// Gets the user id used to identify the control by server.
        /// </summary>
		public string Identity
		{
			get { return strIdentity; }
			internal set { strIdentity = value; }
		}

        /// <summary>
        /// Gets the host name of the irc server.
        /// </summary>
		public string Host
		{
			get { return strHost; }
			internal set { strHost = value; }
		}
	}
}
