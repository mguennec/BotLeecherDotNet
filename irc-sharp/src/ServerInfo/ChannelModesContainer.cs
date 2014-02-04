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
using System.Collections.Specialized;
using System.Text;

namespace ircsharp
{
    /// <summary>
    /// Contains information about channel modes supported by server.
    /// </summary>
    public class ChannelModesContainer : IRCBase
    {
        private ChannelModes _typeA;
        private ChannelModes _typeB;
        private ChannelModes _typeC;
        private ChannelModes _typeD;
        private ChannelModes _all;

        internal ChannelModesContainer(ServerConnection creatorsServerConnection) : base(creatorsServerConnection)
        {
            _typeA = new ChannelModes(creatorsServerConnection);
            _typeB = new ChannelModes(creatorsServerConnection);
            _typeC = new ChannelModes(creatorsServerConnection);
            _typeD = new ChannelModes(creatorsServerConnection);
            _all = new ChannelModes(creatorsServerConnection);
        }

        internal void ParseChanModes(string strChanModes)
        {
            string[] _strModes = strChanModes.Split(',');
            _typeA.SetModes(_strModes[0]);
            _typeB.SetModes(_strModes[1]);
            _typeC.SetModes(_strModes[2]);
            _typeD.SetModes(_strModes[3]);
            _all.SetModes(strChanModes.Replace(",", ""));
        }

        /// <summary>
        /// Gets all channel supported type A modes supported by server.
        /// </summary>
        /// <remarks>Modes that add or remove an address to or from a list. These modes always take a parameter when sent by the server to a client; when sent by a client, they may be specified without a parameter, which requests the server to display the current contents of the corresponding list on the channel to the client.</remarks>
        public ChannelModes TypeA
        {
            get { return _typeA; }
        }

        /// <summary>
        /// Gets all channel supported type B modes supported by server.
        /// </summary>
        /// <remarks>Modes that change a setting on the channel.  These modes always take a parameter.</remarks>
        public ChannelModes TypeB
        {
            get { return _typeB; }
        }

        /// <summary>
        /// Gets all channel supported type C modes supported by server.
        /// </summary>
        /// <remarks>Modes that change a setting on the channel. These modes take a parameter only when set; the parameter is absent when the mode is removed both in the client's and server's MODE command.</remarks>
        public ChannelModes TypeC
        {
            get { return _typeC; }
        }

        /// <summary>
        /// Gets all channel supported type D modes supported by server.
        /// </summary>
        /// <remarks>Modes that change a setting on the channel. These modes never take a parameter.</remarks>
        public ChannelModes TypeD
        {
            get { return _typeD; }
        }

        /// <summary>
        /// Gets all channel modes supported by server.
        /// </summary>
        public ChannelModes All
        {
            get { return _all; }
        }
    }
}
