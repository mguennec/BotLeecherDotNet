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
	/// Summary description for DCCChatContainer.
	/// </summary>
	public class DCCTransferContainer : DCCContainerBase, IEnumerator, IEnumerable
	{
		internal DCCTransferContainer(ServerConnection creatorsServerConnection):base(creatorsServerConnection)
		{
		}

		/// <summary>
		/// Gets a DCC transfer from its array position.
		/// </summary>
		public DCCTransfer this[int Pos]
		{
			get { return GetPos(Pos); }
		}

		/// <summary>
		/// Gets a DCC transfer from its array location.
		/// </summary>
		/// <param name="Pos">The array position to return.</param>
		/// <returns>A DCC transfer.</returns>
		public DCCTransfer GetPos(int Pos)
		{
			if (Pos>=0&&Pos<dccs.Count)
				return (DCCTransfer) dccs[Pos];

			throw(new ArgumentOutOfRangeException("Pos", "Out of range."));
		}

		/// <summary>
		/// Finds a DCC transfer in the array.
		/// </summary>
		/// <param name="ToFind">An DCCTransfer object to find.</param>
		/// <returns>The position of the DCCTransfer object.</returns>
		public int Find(DCCTransfer ToFind)
		{
			for (int x=0;x<dccs.Count;x++)
				if ((DCCTransfer) dccs[x]==ToFind)
					return x;

			throw(new ArgumentException("Item not in list.", "ToFind"));
		}

        #region IEnumerator Members

        public object Current
        {
            get { return (DCCTransfer)dccs[_curr]; }
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
