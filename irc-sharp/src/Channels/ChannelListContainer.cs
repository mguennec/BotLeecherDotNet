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
using ircsharp;

namespace ircsharp
{
	/// <summary>
	/// Summary description for ChannelListContainer.
	/// </summary>
	public class ChannelListContainer : IRCBase, IEnumerable, IEnumerator
	{
		private ArrayList channels = new ArrayList();
        private int _curr = -1;

        /// <summary>
        /// Occurs when the channel list has been fully downloaded.
        /// </summary>
        /// <threadsafe instance="false"/>
        /// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
        /// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
        public event IRCEventHandler ChannelListUpdated;

        /// <summary>
        /// Occurs when the channel list recieves a new item during update.
        /// </summary>
        /// <threadsafe instance="false"/>
        /// <remarks>When this event is fired by the Connection, is is fired in another thread then the original thread
        /// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
        public event ChannelListUpdateProgressEventHandler ChannelListUpdateProgress;

		internal ChannelListContainer(ServerConnection creatorsCurrentConnection) : base(creatorsCurrentConnection)
		{
		}

		/// <summary>
		/// Gets the number of channels in the array.
		/// </summary>
		public int Count
		{
			get { return channels.Count; }
		}

		/// <summary>
		/// Clears the channel list. Useful for freeing up memory.
		/// </summary>
		public void Clear()
		{
			while (channels.Count>0)
				channels.RemoveAt(0);
		}

		/// <summary>
		/// Updates the channel list.
		/// </summary>
		public void Update()
		{
			base.CurrentConnection.SendData("LIST\r\n");
		}

		/// <summary>
		/// Gets a listed channel from the channel name.
		/// </summary>
		public ChannelList this[string ChannelName]
		{
			get { return GetChannel(ChannelName); }
		}

		/// <summary>
		/// Gets a listed channel from its array position.
		/// </summary>
		public ChannelList this[int Pos]
		{
			get { return GetChannel(Pos); }
		}

		/// <summary>
		/// Gets a listed channel from the channel name.
		/// </summary>
		/// <param name="ChannelName">The channel name to find.</param>
		/// <returns>A listed channel.</returns>
		public ChannelList GetChannel(string ChannelName)
		{
			for (int x=0;x<channels.Count;x++)
				if (((ChannelList) channels[x]).Name.ToLower()==ChannelName.ToLower())
					return (ChannelList) channels[x];
			throw(new ArgumentOutOfRangeException("ChannelName", ChannelName + " does not exist."));
		}

		/// <summary>
		/// Gets a listed channel from its array position.
		/// </summary>
		/// <param name="Pos">The array position to return.</param>
		/// <returns>A listed channel.</returns>
		public ChannelList GetChannel(int Pos)
		{
			if (Pos>=0&&Pos<channels.Count)
				return (ChannelList) channels[Pos];
			throw(new ArgumentOutOfRangeException("Pos", "Out of range"));
		}

        public void SortByUserCountDescending()
        {
            ArrayList nChannels = new ArrayList();
            foreach (ChannelList l in channels)
            {
                bool blAdded = false;
                for (int x = 0; x < nChannels.Count; x++)
                {
                    if (((ChannelList) nChannels[x]).UserCount <= l.UserCount)
                    {
                        nChannels.Insert(x, l);
                        blAdded = true;
                        break;
                    }
                }
                if (!blAdded)
                    nChannels.Add(l);
            }

            channels = nChannels;
        }

		internal void AddChannel(ChannelList newChannel)
		{
			channels.Add(newChannel);
		}

        internal void FireProgressEvent(Connection sender, ChannelListUpdateProgress e)
        {
            if (ChannelListUpdateProgress != null)
                ChannelListUpdateProgress(sender, e);
        }

        internal void FireUpdatedEvent(Connection sender, EventArgs e)
        {
            if (ChannelListUpdated != null)
                ChannelListUpdated(sender, e);
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
            get { return channels[_curr]; }
        }

        public bool MoveNext()
        {
            _curr++;
            if (_curr == channels.Count)
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
