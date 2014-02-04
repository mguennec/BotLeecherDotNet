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
	/// This is an abstact class only used within the IRC namespace.
	/// </summary>
	/// <exclude/>
	public abstract class IRCBase
	{
		private ServerConnection _currentConnection;
		
		internal IRCBase(ServerConnection creatorsCurrentConnection)
		{
			_currentConnection = creatorsCurrentConnection;
		}
		
		internal ServerConnection CurrentConnection
		{
			get { return _currentConnection; }
		}
		
		public Connection Connection
		{
			get { return _currentConnection.Owner; }
		}
	} 
}
