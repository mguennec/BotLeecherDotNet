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
using ircsharp;
using System.Collections;

namespace ircsharp
{
	/// <summary>
	/// Summary description for DCCChatContainer.
	/// </summary>
	public class DCCChatContainer : DCCContainerBase, IEnumerator, IEnumerable
	{
		internal DCCChatContainer(ServerConnection creatorsServerConnection):base(creatorsServerConnection)
		{
		}

		/// <summary>
		/// Gets a DCC chat from its array position.
		/// </summary>
		public DCCChat this[int Pos]
		{
			get { return GetPos(Pos); }
		}

		/// <summary>
		/// Gets a DCC chat from its array position.
		/// </summary>
		/// <param name="Pos">The array position to return.</param>
		/// <returns>A DCC Chat</returns>
		public DCCChat GetPos(int Pos)
		{
			if (Pos>=0&&Pos<dccs.Count)
				return (DCCChat) dccs[Pos];

			throw(new ArgumentOutOfRangeException("Pos", "Out of range."));
		}

		/// <summary>
		/// Find a DCC chat.
		/// </summary>
		/// <param name="ToFind">A DCCChat object to find.</param>
		/// <returns>The position of the DCCChat object.</returns>
		public int Find(DCCChat ToFind)
		{
			for (int x=0;x<dccs.Count;x++)
				if ((DCCChat) dccs[x]==ToFind)
					return x;

			throw(new ArgumentException("Item not in list.", "ToFind"));
		}

        #region IEnumerator Members

        public object Current
        {
            get { return (DCCChat)dccs[_curr]; }
        }

        public bool MoveNext()
        {
            _curr++;
            if (_curr == dccs.Count)
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
