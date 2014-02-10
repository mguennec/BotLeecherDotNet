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
    /// Summary description for ChannelContainer.
    /// </summary>
    public class ChannelContainer : IRCBase, IEnumerable, IEnumerator
    {
        private ArrayList channels = new ArrayList();
        private ChannelListContainer channelList;
        private int _curr = -1;

		/// <summary>
		/// Occurs when the client has joined a channel, and all channel information has been retrieved (mode, topic and bans).
		/// </summary>
		/// <seealso cref="JoinCompleteEventArgs"/>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event JoinCompleteEventHandler JoinComplete;
		
		/// <summary>
		/// Occurs when a user send an invitation (/INVITE) to join a channel.
		/// </summary>
		/// <seealso cref="InvitationEventArgs"/>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event InvitationEventHandler Invitation;

		public event PartCompleteEventHandler PartComplete;

        internal ChannelContainer(ServerConnection creatorsCurrentConnection) : base(creatorsCurrentConnection)
        {
            channelList = new ChannelListContainer(creatorsCurrentConnection);
        }

        /// <summary>
        /// Gets the number of channels in the array.
        /// </summary>
        public int Count
        {
            get { return channels.Count; }
        }

        /// <summary>
        /// Gets a channel from its name.
        /// </summary>
        public Channel this[string ChannelName]
        {
            get { return GetChannel(ChannelName); }
        }

        /// <summary>
        /// Gets a channel from its array position.
        /// </summary>
        public Channel this[int Pos]
        {
            get { return GetChannel(Pos); }
        }

        /// <summary>
        /// Gtes a channel from its name.
        /// </summary>
        /// <param name="ChannelName">The channel name to find.</param>
        /// <returns>A channel.</returns>
        public Channel GetChannel(string ChannelName)
        {
            for (int x = 0; x < channels.Count; x++)
                if (((Channel)channels[x]).Name.ToLower() == ChannelName.ToLower())
                    return GetChannel(x);

            return null;
        }

        /// <summary>
        /// Gets a channel from its array position.
        /// </summary>
        /// <param name="Pos">The position in the array to return.</param>
        /// <returns>A channel.</returns>
        public Channel GetChannel(int Pos)
        {
            if (Pos >= 0 && Pos < channels.Count)
                return (Channel)channels[Pos];

            throw (new ArgumentOutOfRangeException("Pos", "Out of range"));
        }

        internal void AddChannel(Channel channel)
        {
            channels.Add(channel);
        }

        internal void RemoveChannel(string ChannelName)
        {
            for (int x = 0; x < channels.Count; x++)
                if (((Channel)channels[x]).Name.ToLower() == ChannelName.ToLower())
                    channels.RemoveAt(x);
        }

        /// <summary>
        /// Joins a channel.
        /// </summary>
        /// <param name="ChannelName">The name of the channel to join.</param>
        public void Join(string ChannelName)
        {
            base.CurrentConnection.SendData("JOIN " + ChannelName + "\r\n");
        }

        /// <summary>
        /// Joins a channel that requires a key to access.
        /// </summary>
        /// <param name="ChannelName">The name of the channel to join.</param>
        /// <param name="Key">The channel key to access the channel.</param>
        public void Join(string ChannelName, string Key)
        {
            base.CurrentConnection.SendData("JOIN " + ChannelName + " :" + Key + "\r\n");
        }

        internal void Clear()
        {
            while (channels.Count > 0)
            {
                ((Channel)channels[0]).Users.Clear();
                ((Channel)channels[0]).Bans.Clear();
                channels.RemoveAt(0);
            }
        }

        /// <summary>
        /// Gets an array of channels availible on the server.
        /// </summary>
        public ChannelListContainer List
        {
            get { return channelList; }
        }

        #region IsOnChannel
        /// <summary>
        /// Checks wheather the Connection is joined a specified channel.
        /// </summary>
        /// <param name="ChannelName">Channel to find.</param>
        /// <returns>Returns true if joined otherwise false.</returns>
        public bool IsOnChannel(string ChannelName)
        {
            for (int x = 0; x < channels.Count; x++)
                if (((Channel)channels[x]).Name.ToLower() == ChannelName.ToLower())
                    return true;
            return false;
        }
		
		internal bool IsUserOnAnyChannel(string Nick, string ExceptChannel)
		{
			foreach (Channel c in channels)
				if (!(ExceptChannel != null && c.Name.ToLower() == ExceptChannel.ToLower()))
					foreach (ChannelUser u in c.Users)
						if (u.User.Nick.ToLower() == Nick.ToLower())
							return true;
			return false;
		}

		internal bool IsUserOnAnyChannel(string Nick)
		{
			return IsUserOnAnyChannel(Nick, null);
		}
        #endregion

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
            if (_curr >= channels.Count)
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
		
		internal void FireJoinCompleted(Channel channel)
		{
			if (this.JoinComplete != null)
				this.JoinComplete(base.CurrentConnection.Owner, new JoinCompleteEventArgs(channel));
		}
		
		internal void FirePartCompleted(Channel channel)
		{
			if (this.PartComplete != null)
				this.PartComplete(base.CurrentConnection.Owner, new PartCompleteEventArgs(channel));
		}
		
		internal void FireInvitation(User user, string channelName)
		{
			if (this.Invitation != null)
				this.Invitation(base.CurrentConnection.Owner, new InvitationEventArgs(user, channelName));
		}
	}
}