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
	public partial class Connection : IRCBase
	{
		#region Events
		/// <summary>
		/// Occurs when trying to connect to an IRC server and fails.
		/// </summary>
		/// <seealso cref="ConnectFailedEventArgs"/>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event ConnectFailedEventHandler ConnectFailed;

		/// <summary>
		/// Occurs when the IRC control has succesfully connected and the MOTD has been retrieved.
		/// </summary>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event IRCEventHandler Connected;

		/// <summary>
		/// Occurs when the server disconnects.
		/// </summary>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event IRCEventHandler Disconnected;

		/// <summary>
		/// Occurs when server disconnects because of an error.
		/// </summary>
		/// <seealso cref="ErrorEventArgs"/>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event ErrorEventHandler Error;

		/// <summary>
		/// Occurs when the library need your client information. If not implemented, the client will return "TNets IRC Library v.0.7" 
		/// </summary>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event CtCpVersionReplyEventHandler CtCpVersionNeedsClientInfo;
	
		/// <summary>
		/// Occurs when the server has confirmed your outside IP, and the LocalIP is updated.
		/// </summary>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event IRCEventHandler LocalIPUpdated;
		
		/// <summary>
		/// Occurs when the server cannot connect because nick already is in use on server. Return new nick.
		/// </summary>
		public event NickInUseEventHandler NickNameInUse;
		#endregion
	}
}
