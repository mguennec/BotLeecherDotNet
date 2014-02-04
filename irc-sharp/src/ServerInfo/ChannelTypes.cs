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
using System.Text;

namespace ircsharp
{
    public class ChannelTypes : IRCBase
    {
        string _strChannelPrefixs = "#";
        string _strChannelLimits = "#:";

        private Connection _parent;

        internal ChannelTypes(ServerConnection creatorsServerConnection, Connection parent) : base(creatorsServerConnection)
        {
            _parent = parent;
        }

        internal void SetTypes(string strChannelPrefixs, string strChannelLimits)
        {
            _strChannelPrefixs = strChannelPrefixs;
            _strChannelLimits = strChannelLimits;
        }

        /// <summary>
        /// Returns true if specified channel prefix is supported by server.
        /// </summary>
        /// <param name="ChannelPrefix">Channel prefix to get status for.</param>
        /// <returns>Boolean. True if channel prefix is supported by server.</returns>
        public bool this[char ChannelPrefix]
        {
            get { return _strChannelPrefixs.IndexOf(ChannelPrefix) != -1; }
        }

        /// <summary>
        /// Gets a string containing all prefixes supported by server.
        /// </summary>
        public string AllPrefixes
        {
            get { return _strChannelPrefixs; }
        }

        private string[] GetPrefix(char ChannelPrefix)
        {
            string[] strPrefixs = _strChannelLimits.Split(',');
            foreach (string strPrefix in strPrefixs)
            {
                string strP = strPrefix;
                if (strP == "#" && _parent.Server["MAXCHANNELS"] != null)
                    strP = string.Format("#:{0}", _parent.Server["MAXCHANNELS"]);
                string[] strParts = strP.Split(':');
                if (strParts.Length > 1 && strParts[1] == "")
                    strParts[1] = "0";
                if (strParts.Length == 1)
                    strParts = new string[] {strParts[0], "0"};
                if (strParts[0].IndexOf(ChannelPrefix) != -1)
                    return strParts;
            }

            return null;
        }

        /// <summary>
        /// Gets an integer containing the channel limit for a specified channel prefix.
        /// </summary>
        /// <param name="ChannelPrefix">The channel prefix to get the limit for.</param>
        /// <returns>The maximum number of joined channels supported by server from specified channel prefix. Returns 0 (zero) if unlimited, -1 if unknown.</returns>
        public int GetLimitByPrefix(char ChannelPrefix)
        {
            string[] strParts = GetPrefix(ChannelPrefix);
            if (strParts != null)
            {
                if (strParts[0].IndexOf(ChannelPrefix) != -1)
                    return int.Parse(strParts[1]);
            }

            return -1;
        }

        /// <summary>
        /// Gets the number of channels left to join with the specified channel prefix.
        /// </summary>
        /// <param name="ChannelPrefix">Channel prefix</param>
        /// <returns>Integer. Number of channels left to join with specified channel prefix.</returns>
        /// <remarks>Finds all channels already joined with specified prefix, and substract it from the total number of allowed channels on server.</remarks>
        public int GetChannelsLeftToJoinByPrefix(char ChannelPrefix)
        {
            string[] strParts = GetPrefix(ChannelPrefix);
            if (strParts != null)
            {
                if (int.Parse(strParts[1]) > 0)
                {
                    int intCurrentChannels = 0;
                    foreach (Channel c in _parent.Channels)
                        if (strParts[0].IndexOf(c.Name[0]) != -1)
                            intCurrentChannels++;

                    return int.Parse(strParts[1]) - intCurrentChannels;
                }
                return 0;
            }

            return -1;
        }
    }
}
