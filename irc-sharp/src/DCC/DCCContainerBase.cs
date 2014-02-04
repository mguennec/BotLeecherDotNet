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
	public abstract class DCCContainerBase : IRCBase
	{
		protected ArrayList dccs = new ArrayList();
        protected int _curr = -1;

		internal DCCContainerBase(ServerConnection creatorsServerConnection) : base(creatorsServerConnection)
		{
		}

		/// <summary>
		/// Gets the number of DCCs in the array.
		/// </summary>
		public int Count
		{
			get { return dccs.Count; }
		}

		internal void AddDcc(DCCBase newDcc)
		{
			dccs.Add(newDcc);
		}

		internal void Remove(int Pos)
		{
			dccs.RemoveAt(Pos);
		}

		internal void Remove(DCCBase toRemove)
		{
			dccs.Remove(toRemove);
		}
    }
}
