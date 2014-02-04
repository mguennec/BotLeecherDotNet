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
using System.Timers;
using ircsharp;

namespace ircsharp
{
	internal class DccStateObject
	{
		internal const int BufferSize = 8192;
		internal byte[] Buffer = new byte[BufferSize];
	}

	/// <summary>
	/// Class used for DCC transfers and DCC chats. This class can only be instanced by Connection.
	/// </summary>
	public abstract class DCCBase : IRCBase
	{
		/// <summary>
		/// Internal variable.
		/// </summary>
		protected Socket socket;
		private Socket listenSocket;
		/// <summary>
		/// Internal variable.
		/// </summary>
		protected IPEndPoint RemoteEndPoint;
		
		/// <summary>
		/// Internal variable.
		/// </summary>
		protected string strNickname;

		private Timer timer;

		public event EventHandler EtablishTimeout;
		public event EventHandler EtablishFailed;

		private DCCContainerBase container;

		private int intID = 0;

		internal DCCBase(ServerConnection creatorsServerConnection, long RemoteHost, int Port, string Nickname, DCCContainerBase parent) : base(creatorsServerConnection)
		{
			container = parent;
			intID = Port;
			IPAddress ip = GetIP(RemoteHost);
			RemoteEndPoint = new IPEndPoint(ip, Port);
			strNickname = Nickname;
		}

		internal DCCBase(ServerConnection creatorsServerConnection, string Nickname, int Port, DCCContainerBase parent) : base(creatorsServerConnection)
		{
			strNickname = Nickname;
			intID = Port;
			container = parent;
            listenSocket = new Socket(IPAddress.Loopback.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			timer = new Timer(60000);
			timer.Elapsed += new ElapsedEventHandler(TimerProc);
			timer.AutoReset = true;
			timer.Enabled = true;
			listenSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
			listenSocket.Listen(100);
			listenSocket.BeginAccept(new AsyncCallback(acceptCallback), null);
		}

		private IPAddress GetIP(long Host)
		{
			long[] ips = new long[] {0,0,0,0};
			ips[0] = (long) Math.Floor((double)Host / (256*256*256));
			Host = Host - (ips[0] * (256*256*256));
			ips[1] = (long) Math.Floor((double)Host / (256*256));
			Host = Host - (ips[1] * (256*256));
			ips[2] = (long) Math.Floor((double)Host / 256);
			Host = Host - (ips[2] * 256);
			ips[3] = Host;

			string strIP = ips[0].ToString() + "." + ips[1].ToString() + "." + ips[2].ToString() + "." + ips[3].ToString();

            return IPAddress.Parse(strIP);
            //return new IPAddress(Host);
			//return Dns.GetHostEntry(strIP).AddressList[0];
		}

		/// <summary>
		/// Internal function.
		/// </summary>
		protected void EtablishConnection()
		{
			socket = new Socket(IPAddress.Loopback.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			socket.BeginConnect(RemoteEndPoint, new AsyncCallback(connectCallback), null);
		}

		/// <summary>
		/// Internal function
		/// </summary>
		protected void CloseSocket()
		{
			if (socket!=null&&socket.Connected)
			{
				socket.Shutdown(SocketShutdown.Both);
				socket.Close();
			}
			container.Remove(this);
		}

		/// <summary>
		/// Internal function.
		/// </summary>
		/// <param name="toSend"></param>
		protected void SendData(string toSend)
		{
			SendData(System.Text.Encoding.Default.GetBytes(toSend), false);
		}

		/// <summary>
		/// Internal function
		/// </summary>
		/// <param name="toSend"></param>
		protected void SendData(byte[] toSend)
		{
			SendData(toSend, false);
		}

		/// <summary>
		/// Internal function.
		/// </summary>
		/// <param name="toSend"></param>
		/// <param name="blCloseAfterSend"></param>
		protected void SendData(byte[] toSend, bool blCloseAfterSend)
		{
			if (socket.Connected)
				socket.BeginSend(toSend, 0, toSend.Length, SocketFlags.None, new AsyncCallback(sendCallback), blCloseAfterSend);
		}

		/// <summary>
		/// Internal function.
		/// </summary>
		protected abstract void Connected();
		protected abstract void OnData(byte[] buffer, int Length);
		protected abstract void Disconnected();

		/// <summary>
		/// Gets the nickname of the remote user.
		/// </summary>
		public string Nick
		{
			get { return strNickname; }
		}

		/// <summary>
		/// Gets wheather the DCC is etablished.
		/// </summary>
		public bool Etablished
		{
			get { return socket.Connected; }
		}

		internal int Identifier
		{
			get { return intID; }
		}

		private void TimerProc(object sender, ElapsedEventArgs e)
		{
			listenSocket.Close();
			if (EtablishTimeout!=null)
				EtablishTimeout(this, new EventArgs());
			container.Remove(this);
		}

		#region Socket Async functions
    	private void connectCallback(IAsyncResult ar)
		{
            try
            {
                socket.EndConnect(ar);
            }
            catch
            {
                if (EtablishFailed != null)
                    EtablishFailed(this, new EventArgs());
                return;
            }
			
			if (socket.Connected)
			{
				DccStateObject dso = new DccStateObject();
				socket.BeginReceive(dso.Buffer, 0, DccStateObject.BufferSize, SocketFlags.None, new AsyncCallback(receiveCallback), dso);
				Connected();
			}
			else
			{
				CloseSocket();
				if (EtablishFailed!=null)
					EtablishFailed(this, new EventArgs());
				container.Remove(this);
			}
		}

		private void acceptCallback(IAsyncResult ar)
		{
			try
			{
				timer.Enabled = false;
				socket = listenSocket.EndAccept(ar);
				DccStateObject dso = new DccStateObject();
				socket.BeginReceive(dso.Buffer, 0, DccStateObject.BufferSize, SocketFlags.None, new AsyncCallback(receiveCallback), dso);
				listenSocket.Close();
				if (socket.Connected)
					Connected();
				else
				{
					if (EtablishFailed!=null)
						EtablishFailed(this, new EventArgs());
					container.Remove(this);
				}
			}
			catch
			{
				if (EtablishFailed!=null)
					EtablishFailed(this, new EventArgs());
				container.Remove(this);
			}
		}

		private void receiveCallback(IAsyncResult ar)
		{
			DccStateObject dso = (DccStateObject) ar.AsyncState;

			try
			{
				int intRead = socket.EndReceive(ar);
				if (intRead>0)
				{
					OnData(dso.Buffer, intRead);
					socket.BeginReceive(dso.Buffer, 0, DccStateObject.BufferSize, SocketFlags.None, new AsyncCallback(receiveCallback), dso);
				}
				else
				{
					CloseSocket();
					Disconnected();
					container.Remove(this);
				}
			}
			catch
			{
				CloseSocket();
				Disconnected();
				container.Remove(this);
			}
		}

		private void sendCallback(IAsyncResult ar)
		{
			if ((bool) ar.AsyncState)
			{
				Disconnected();
				CloseSocket();
			}
		}
		#endregion
    }
}
