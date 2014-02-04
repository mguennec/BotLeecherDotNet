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
using System.Collections;

namespace ircsharp
{
	/// <summary>
	/// Summary description for ChannelUserContainer.
	/// </summary>
	public class ChannelUserContainer : IRCBase, IEnumerable, IEnumerator
	{
		private ArrayList users = new ArrayList();
        private int _curr = -1;

		internal ChannelUserContainer(ServerConnection creatorsCurrentConnection) : base(creatorsCurrentConnection)
		{
		}

		internal void AddUser(ChannelUser newUser)
		{
			users.Add(newUser);
		}

		internal void RemoveUser(string Nickname)
		{
			for (int x=0;x<users.Count;x++)
				if (((ChannelUser) users[x]).User.Nick.ToLower()==Nickname.ToLower())
					users.RemoveAt(x);
		}

		internal void RemoveUser(int Pos)
		{
			try	{ users.RemoveAt(Pos); }
			catch {}
		}

		/// <summary>
		/// Gets the number of users in the array.
		/// </summary>
		public int Count
		{
			get { return users.Count; }
		}
		
		/// <summary>
		/// Gets a user from his/her nick name.
		/// </summary>
		public ChannelUser this[string Nickname]
		{
			get { return GetUser(Nickname); }
		}
		
		public ChannelUser this[User theUser]
		{
			get { return GetUser(theUser.Nick); }
		}

		/// <summary>
		/// Gets a user from his/her array position.
		/// </summary>
		public ChannelUser this[int Pos]
		{
			get { return GetUser(Pos); }
		}

		/// <summary>
		/// Gets a user from his/her nick name.
		/// </summary>
		/// <param name="Nickname">The nick name to find.</param>
		/// <returns>A channel user.</returns>
		public ChannelUser GetUser(string Nickname)
		{
			for (int x=0;x<users.Count;x++)
				if (((ChannelUser) users[x]).User.Nick.ToLower()==Nickname.ToLower())
					return GetUser(x);
			throw(new ArgumentOutOfRangeException("Nickname", Nickname + " does not exist."));
		}
		
		public ChannelUser GetUser(User theUser)
		{
			return GetUser(theUser.Nick);
		}

		/// <summary>
		/// Gets a user from his/her array position.
		/// </summary>
		/// <param name="Pos">The position of the user in the array.</param>
		/// <returns>A channel user.</returns>
		public ChannelUser GetUser(int Pos)
		{
			if (Pos>=0&&Pos<users.Count)
				return (ChannelUser) users[Pos];
			throw(new ArgumentOutOfRangeException("Pos", "Out of range."));
		}

		/// <summary>
		/// Returns wheather a certain nick is on the channel.
		/// </summary>
		/// <param name="Nickname"></param>
		/// <returns></returns>
		public bool IsUserOnChannel(string Nickname)
		{
			for (int x=0;x<users.Count;x++)
				if (((ChannelUser) users[x]).User.Nick.ToLower()==Nickname.ToLower())
					return true;
			return false;
		}

		internal void Clear()
		{
			while (users.Count>0)
				users.RemoveAt(0);
		}

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this;
        }

        #endregion

        #region IEnumerator Members

        public object Current
        {
            get { return users[_curr]; }
        }

        public bool MoveNext()
        {
            _curr++;
            if (_curr == users.Count)
            {
                Reset();
                return false;
            }
            return true; ;
        }

        public void Reset()
        {
            _curr = -1;
        }

        #endregion
}
}
