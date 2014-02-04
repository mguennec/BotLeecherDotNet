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
using System.Collections;

namespace ircsharp
{
	public class UserContainer : IRCBase, IEnumerator, IEnumerable
	{
		ArrayList users;
		string strOwnNick;
		int _curr;
		
		public event UnknownUserEmergedEventHandler UnknownUserEmerged;
		
		internal UserContainer(ServerConnection creatorsServerConnection) : base(creatorsServerConnection)
		{
			users = new ArrayList();
			_curr = -1;
		}
		
		internal string OwnNick
		{
			set { strOwnNick = value; }
		}
		
		internal User AddUser(User newUser)
		{
			users.Add(newUser);
			return newUser;
		}
		
		public User Me
		{
			get { return GetUser(strOwnNick); }
		}
		
		internal void Clear()
		{
			while (users.Count > 0)
				users.RemoveAt(0);
		}
		
		internal void Remove(string Nick)
		{
			foreach (User u in users)
			{
				if (u.Nick.ToLower() == Nick.ToLower())
				{
					users.Remove(u);
					break;
				}
			}
		}
		
		public int Count
		{
			get { return users.Count; }
		}
		
		internal User GetUser(string Nick, bool askWho)
		{
			foreach (User u in users)
				if (u.Nick.ToLower() == Nick.ToLower())
					return u;
			
			User ret = new User(base.CurrentConnection, Nick, askWho);
			users.Add(ret);
			if (this.UnknownUserEmerged != null)
				this.UnknownUserEmerged(base.CurrentConnection.Owner, new UnknownUserEmergedEventArgs(ret));
			return ret;
		}
		
		public User GetUser(string Nick)
		{
			return GetUser(Nick, true);
		}
		
		internal User GetUser(UserInfo user)
		{
			foreach (User u in users)
				if (u.Nick.ToLower() == user.Nick.ToLower())
					return u;
			
			User ret = new User(base.CurrentConnection, user.Nick, user.Identity, user.Host, true);
			users.Add(ret);
			return ret;
		}
				
		public User GetUser(int Pos)
		{
			if (Pos < 0 && Pos >= users.Count)
				throw(new ArgumentOutOfRangeException("Pos", "Out of range"));
			
			return (User) users[Pos];
		}
		
		public User this[string Nick]
		{
			get { return GetUser(Nick); }
		}
		
		public User this[int Pos]
		{
			get { return GetUser(Pos); }
		}
		
		public IEnumerator GetEnumerator()
		{
			return this;
		}
		
		public object Current
		{
			get { return users[_curr]; }
		}
		
		public void Reset()
		{
			_curr = -1;
		}
		
		public bool MoveNext()
		{
			_curr++;
			if (_curr == users.Count)
			{
				Reset();
				return false;
			}
			return true;
		}
	}
}
