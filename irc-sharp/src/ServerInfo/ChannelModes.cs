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
    public class ChannelModes : IRCBase
    {
        private string _strModes = "";

        internal ChannelModes(ServerConnection creatorsServerConnection) : base(creatorsServerConnection)
        {
        }

        internal void SetModes(string strModes)
        {
            _strModes = strModes;
        }

        public bool this[char ModeChar]
        {
            get { return _strModes.IndexOf(ModeChar) != -1; }
        }

        public bool IsSupported(char ModeChar)
        {
            return _strModes.IndexOf(ModeChar) != -1;
        }

        public string AllSupported
        {
            get { return _strModes; }
        }
    }
}
