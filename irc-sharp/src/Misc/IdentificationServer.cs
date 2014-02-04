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
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using ircsharp;

namespace ircsharp
{
	internal class IRCIdentStateObject
	{
		internal Socket socket;
		internal const int bufferSize = 1024;
		internal byte[] buffer = new byte[bufferSize];

		internal IRCIdentStateObject(Socket newSocket)
		{
			socket = newSocket;
		}
	}

	/// <summary>
	/// Summary description for IdentificationServer.
	/// </summary>
	public class IdentificationServer : IRCBase
	{
		private bool blEnabled = false;
		private bool blListen = false;
		private string strOS;
		private Connection ircParent;

		public event IRCEventHandler ResponseSent;

		Socket listenSocket = null;

		internal IdentificationServer(ServerConnection creatorsServerConnection, Connection parent) : base(creatorsServerConnection)
		{
			ircParent = parent;
			this.OS = null; // Forces auto-detection
		}

		internal bool Listen
		{
			get { return blListen; }
			set
			{ 
				blListen = value;
				SetEnabled(blListen&&blEnabled&&ircParent.IsConnecting);
			}
		}

		/// <summary>
		/// Gets or sets the OS to reply the IRC server with.
		/// </summary>
		public string OS
		{
			get { return strOS; }
			set
			{
				if (value!=null&&value.Trim()!="")
					strOS = value;
				else
				{
					switch (Environment.OSVersion.Platform)
					{
					case PlatformID.Unix:
						strOS = "UNIX";
						break;
					case PlatformID.Win32NT:
					case PlatformID.Win32Windows:
						strOS = "WIN32";
						break;
					case PlatformID.Win32S:
						strOS = "WIN16";
						break;
					case PlatformID.WinCE:
						strOS = "WINCE";
						break;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets wheather the IdentD server is enabled. Default is false.
		/// </summary>
		public bool Enabled
		{
			get { return blEnabled; }
			set
			{
				blEnabled = value;
				SetEnabled(blListen&&blEnabled&&ircParent.IsConnecting);
			}
		}

		private void SetEnabled(bool blWay)
		{
			if (blWay)
			{
				listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				try
				{
					listenSocket.Bind(new IPEndPoint(IPAddress.Any, 113));
					listenSocket.Listen(100);
					listenSocket.BeginAccept(new AsyncCallback(incomingConnection), null);
				}
				catch
				{}
			}
			else
			{
				if (listenSocket!=null)
				{
					listenSocket.Close();
					listenSocket = null;
				}
			}
		}

		private bool CompareAddressBytes(byte[] byte1, byte[] byte2)
		{
			if (byte1.Length != byte2.Length)
				return false;
			
			for (int x = 0 ; x < byte1.Length ; x++)
				if (byte1[x] != byte2[x])
					return false;
			
			return true;
		}

		private void incomingConnection(IAsyncResult ar)
		{
            if (listenSocket != null)
            {
                Socket newSocket = listenSocket.EndAccept(ar);
                this.Listen = false;

                if (newSocket != null && newSocket.Connected)
                {
                    if (CompareAddressBytes(((IPEndPoint)newSocket.RemoteEndPoint).Address.GetAddressBytes(), base.CurrentConnection.RemoteIP))
                    {
                        IRCIdentStateObject so = new IRCIdentStateObject(newSocket);
                        newSocket.BeginReceive(so.buffer, 0, IRCIdentStateObject.bufferSize, SocketFlags.None, new AsyncCallback(receiveCallback), so);
                    }
                    else
                    {
                        newSocket.Shutdown(SocketShutdown.Both);
                        newSocket.Close();
                        newSocket = null;
                    }
                }
            }
		}

		private void receiveCallback(IAsyncResult ar)
		{
			IRCIdentStateObject so = (IRCIdentStateObject) ar.AsyncState;
			
			int intRead = so.socket.EndReceive(ar);

			if (so.socket.Connected)
			{
				if (intRead>0)
				{
					string strData = Encoding.Default.GetString(so.buffer, 0, intRead);
					strData = strData.Replace("\r", "").Replace("\n", "");
					if (strData.IndexOf(",")>-1)
					{
						string[] strPorts = strData.Split(new char[] {','});
						if (strPorts.Length==2&&strPorts[0].Trim()==base.CurrentConnection.LocalPort.ToString()&&strPorts[1].Trim()==base.CurrentConnection.RemotePort.ToString())
							SendData(strData + " : Identity : " + strOS + " : " + ircParent.Identity , so.socket, true);
						else
							SendData(strData + " : ERROR : NO-USER", so.socket, false);
					}
					else
					{
						so.socket.Shutdown(SocketShutdown.Both);
						so.socket.Close();
						so.socket = null;
					}
				}
				else
				{
					so.socket.Shutdown(SocketShutdown.Both);
					so.socket.Close();
					so.socket = null;
				}
			}
		}

		private void SendData(string strData, Socket socket, bool succes)
		{
			byte[] tmp = Encoding.Default.GetBytes(strData);
			if (socket.Connected)
			{
				socket.BeginSend(tmp, 0, tmp.Length, SocketFlags.None, new AsyncCallback(sendComplete), socket);
				if (succes&&ResponseSent!=null)
					ResponseSent(ircParent, new EventArgs());
			}
		}

		private void sendComplete(IAsyncResult ar)
		{
			Socket socket = (Socket) ar.AsyncState;
			if (socket.Connected)
			{
				socket.Shutdown(SocketShutdown.Both);
				socket.Close();
				socket = null;
			}
		}
	}
}
