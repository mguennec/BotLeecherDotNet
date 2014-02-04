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
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace ircsharp
{
    public class ServerInfo : IRCBase, IEnumerator, IEnumerable
    {
		/// <summary>
		/// Occurs when the servers sends a server message.
		/// </summary>
		/// <seealso cref="ServerMessageEventArgs"/>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event ServerMessageEventHandler ServerMessage;

		/// <summary>
		/// Occurs when user sends PING? request and the IRC control replyes with a PONG!.
		/// </summary>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event IRCEventHandler PingPong;

		// TODO: Move to the ServerInfo class
		/// <summary>
		/// Occurs when the servers Message of the Day is updated.
		/// </summary>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event IRCEventHandler MessageOfTheDayUpdated;

		/// <summary>
		/// Occurs when the server sends a notice (/SNOTICE).
		/// </summary>
		/// <seealso cref="ServerNoticeEventArgs"/>
		/// <threadsafe instance="false"/>
		/// <remarks>When this event is fired by the Connection, it is fired in another thread then the original thread 
		/// that created the class. Because of this you need to use the Invoke() method on your form if you're creating using Windows forms.</remarks>
		public event ServerNoticeEventHandler ServerNotice;
		public event RawDataEventHandler RawRecieve;

        /// <summary>
        /// Identifies case mappings used by server.
        /// </summary>
        public enum IRCCaseMapping
        {
            ASCII,
            RFC1459,
            RFC1459strict
        }

        private NameValueCollection nm = new NameValueCollection();
        private ChannelModesContainer cmc;
        private ChannelTypes ct;
        private int _curr = -1;
		private string strMotd;

        private string _prefixStatus = "ov";
        private string _prefix = "@+";

        internal ServerInfo(ServerConnection creatorsServerConnection, Connection parent) : base(creatorsServerConnection)
        {
            cmc = new ChannelModesContainer(creatorsServerConnection);
            ct = new ChannelTypes(creatorsServerConnection, parent);
        }

        internal void Parse005(string[] strMessage, int descriptionBeginsAtIndex)
        {
            for (int x = 0; x < descriptionBeginsAtIndex; x++)
            {
                string strParam = strMessage[x].Split('=')[0];
                string strValue = "";
                if (strMessage[x].IndexOf('=') != -1)
                    strValue = strMessage[x].Split('=')[1];

                if (strParam.Length > 0)
                {
                    if (strParam[0] == '-')
                    {
                        if (nm[strParam.Substring(1)] != null)
                            nm.Remove(strParam.Substring(1));
                    }
                    else
                    {
                        if (nm[strParam] != null)
                            nm[strParam] = strValue;
                        else
                            nm.Add(strParam, strValue);

                        // Parsing those that are frequently used, to speed up peformance.
                        switch (strParam)
                        {
                            case "PREFIX":
                                ParsePrefix(strValue);
                                break;
                            case "CHANMODES":
                                cmc.ParseChanModes(strValue);
                                break;
                            case "CHANLIMIT":
                            case "CHANTYPES":
                                if (nm["CHANTYPES"] != null && (nm["CHANLIMIT"] != null || nm["MAXCHANNELS"] != null))
                                {
                                    string strTypes = nm["CHANTYPES"];
                                    string strLimit = string.Format("{0}:{1}", strTypes, nm["MAXCHANNELS"]);
                                    if (nm["CHANLIMIT"] != null)
                                        strLimit = nm["CHANLIMIT"];

                                    ct.SetTypes(strTypes, strLimit);
                                }
                                break;
                        }
                    }
                }
            }
        }

        #region Parsers
        private void ParsePrefix(string strValue)
        {
            Regex re = new Regex("\\((.*?)\\)(.*)");
            Match m = re.Match(strValue);
            _prefix = m.Groups[2].Value;
            _prefixStatus = m.Groups[1].Value;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of the network that the server is attached to.
        /// </summary>
        public string NetworkName
        {
            get { return nm["NETWORK"]; }
        }

        /// <summary>
        /// Returns case mapping allowed by server in channel names and user names.
        /// </summary>
        public IRCCaseMapping CaseMapping
        {
            get
            {
                switch (nm["CASEMAPPING"])
                {
                    case "ascii":
                        return IRCCaseMapping.ASCII;
                    case "strick-rfc1459":
                        return IRCCaseMapping.RFC1459strict;
                    case "rfc1459":
                    default:
                        return IRCCaseMapping.RFC1459;
                }
            }
        }

        /// <summary>
        /// Gets a user status from the prefix character used by server.
        /// </summary>
        /// <param name="Prefix">The chacacter to get status for.</param>
        /// <returns>An ChannelUserStatus value.</returns>
        public ChannelUserStatus GetUserStatusFromPrefix(char Prefix)
        {
            if (_prefix.IndexOf(Prefix) != -1)
            {
                switch (_prefixStatus[_prefix.IndexOf(Prefix)])
                {
                    case 'o':
                        return ChannelUserStatus.Operator;
                    case 'v':
                        return ChannelUserStatus.Voiced;
                    case 'h':
                        return ChannelUserStatus.HalfOperator;
                    default:
                        return ChannelUserStatus.UnknownStatus;
                }
            }
            return ChannelUserStatus.NotAStatus;
        }

        /// <summary>
        /// Gets the characters users with specified status has.
        /// </summary>
        /// <param name="Status">Status to get character for.</param>
        /// <returns>The character used to identify users with specified status on server.</returns>
        public char GetPrefixFromUserStatus(ChannelUserStatus Status)
        {
            switch (Status)
            {
                case ChannelUserStatus.Operator:
                    return _prefixStatus.IndexOf('o') != -1 ? _prefix[_prefixStatus.IndexOf('o')] : '@';
                case ChannelUserStatus.HalfOperator:
                    return _prefixStatus.IndexOf('h') != -1 ? _prefix[_prefixStatus.IndexOf('h')] : '%';
                case ChannelUserStatus.Voiced:
                    return _prefixStatus.IndexOf('v') != -1 ? _prefix[_prefixStatus.IndexOf('v')] : '+';
            }

            return (char) 0;
        }

        /// <summary>
        /// Gets the character used to identify operators on channels.
        /// </summary>
        /// <remarks>Default is '@'.</remarks>
        public char OperatorPrefix
        {
            get { return _prefix[_prefixStatus.IndexOf('o')]; }
        }

        /// <summary>
        /// Gets the character used to identify voiced users on channels.
        /// </summary>
        /// <remarks>Default is '+'.</remarks>
        public char VoicePrefix
        {
            get { return _prefix[_prefixStatus.IndexOf('v')]; }
        }

        /// <summary>
        /// Gets information on supported modes by server.
        /// </summary>
        public ChannelModesContainer ChannelModes
        {
            get { return cmc; }
        }

        /// <summary>
        /// Gets information on different channel types and their limits.
        /// </summary>
        public ChannelTypes ChannelTypes
        {
            get { return ct; }
        }

        /// <summary>
        /// Returns true if the client can request a channel list, without worrying disconnection because of the mass amount of data.
        /// </summary>
        public bool SafeChannelList
        {
            get { return nm["SAFELIST"] != null; }
        }

        /// <summary>
        /// Gets the maximum number of characters the nick can have according to server rules. Returns -1 if unknown.
        /// </summary>
        /// <remarks>According to protocol specifications, in case this value is unknown, one should assume the maximum length is 9.</remarks>
        public int MaximumNickLength
        {
            get
            {
                if (nm["NICKLEN"] != null)
                    return int.Parse(nm["NICKLEN"]);
                return -1;
            }
        }

		public void RawSend(string data)
		{
			base.CurrentConnection.SendData(data);
		}
		
        #endregion

		public string MessageOfTheDay
		{
			get { return strMotd; }
			internal set { strMotd = value; }
		}
		
		internal void FireMessageOfTheDayUpdated(Connection sender)
		{
			if (MessageOfTheDayUpdated != null)
				MessageOfTheDayUpdated(sender, new EventArgs());
		}
		
        #region Array access methods
        public string Get(string Value)
        {
            return nm[Value];
        }

        public string Get(int Pos)
        {
            return nm[Pos];
        }

        public int Count
        {
            get { return nm.Count; }
        }

        public string this[string Value]
        {
            get { return nm[Value]; }
        }

        public string this[int Pos]
        {
            get { return nm[Pos]; }
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
            get { return nm[_curr]; }
        }

        public bool MoveNext()
        {
            _curr++;
            if (_curr == nm.Count)
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
		
		internal void FireServerMessage(int serverCode, string[] parameters)
		{
			if (this.ServerMessage != null)
				this.ServerMessage(base.CurrentConnection.Owner, new ServerMessageEventArgs(serverCode, parameters));
		}
		
		internal void FirePingPong()
		{
			if (this.PingPong != null)
				this.PingPong(base.CurrentConnection.Owner, new EventArgs());
		}
		
		internal void FireServerNotice(string recipient, string message)
		{
			if (this.ServerNotice != null)
				this.ServerNotice(base.CurrentConnection.Owner, new ServerNoticeEventArgs(recipient, message));
		}
		
		internal void FireRawRecieve(string data)
		{
			if (this.RawRecieve != null)
				this.RawRecieve(base.CurrentConnection.Owner, new RawDataEventArgs(data));
		}
    }
}
