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
	/// Summary description for ChannelBanContainer.
	/// </summary>
	public class ChannelBanContainer : IRCBase, IEnumerator, IEnumerable
	{
		private ArrayList bans = new ArrayList();
        private int _curr = -1;

		internal ChannelBanContainer(ServerConnection creatorsCurrentConnection) : base(creatorsCurrentConnection)
		{
		}

		/// <summary>
		/// Gets the number of items in the array.
		/// </summary>
		public int Count
		{
			get { return bans.Count; }
		}

		/// <summary>
		/// Gets a channel ban from an array position.
		/// </summary>
		public ChannelBan this[int Pos]
		{
			get { return GetBan(Pos); }
		}

		/// <summary>
		/// Gets a channel ban from the host mask.
		/// </summary>
		public ChannelBan this[string Hostmask]
		{
			get { return GetBan(Hostmask); }
		}

		/// <summary>
		/// Gets a channel ban from an array position.
		/// </summary>
		/// <param name="Pos">The array position.</param>
		/// <returns>A channel ban.</returns>
		public ChannelBan GetBan(int Pos)
		{
			if (Pos>=0&&Pos<bans.Count)
				return (ChannelBan) bans[Pos];
			throw(new ArgumentOutOfRangeException("Pos", "Out of range."));
		}

		/// <summary>
		/// Gets a channel ban from a host mask.
		/// </summary>
		/// <param name="Hostmask">The host mask to find.</param>
		/// <returns>A channel ban.</returns>
		public ChannelBan GetBan(string Hostmask)
		{
			for (int x=0;x<bans.Count;x++)
				if (((ChannelBan) bans[x]).HostMask.ToLower()==Hostmask.ToLower())
					return (ChannelBan) bans[x];
			throw(new ArgumentOutOfRangeException("Hostmask", Hostmask + " does not exist."));
		}

		internal void AddBan(ChannelBan newBan)
		{
			bans.Add(newBan);
		}

		internal void RemoveBan(string Hostname)
		{
			for (int x=0;x<bans.Count;x++)
				if (((ChannelBan) bans[x]).HostMask.ToLower()==Hostname.ToLower())
					bans.RemoveAt(x);
		}

		internal void Clear()
		{
			while (bans.Count>0)
				bans.RemoveAt(0);
		}

        #region IEnumerator Members

        public object Current
        {
            get { return bans[_curr]; }
        }

        public bool MoveNext()
        {
            _curr++;
            if (_curr == bans.Count)
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

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this;
        }

        #endregion
    }
}
