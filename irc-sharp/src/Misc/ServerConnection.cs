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
#region Using
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
#endregion

namespace ircsharp
{
	#region IRCStateObject
	internal class IRCStateObject
	{
		public const int bufferSize = 4096;
		public byte[] buffer = new byte[bufferSize];
		public string lastBuffer = "";
	}
	#endregion
	#region OnDataEventArgs class
	internal class OnDataEventArgs
	{
		private string strUser = "";
		private string strCommand = "";
		private string[] strParameters;
		private bool blIsServerMessage;
		private int intdescriptionBeginsAtIndex;
		private string strRaw;

		internal OnDataEventArgs(string user, string command, string[] parameters, int descriptionBeginsAtIndex, bool isServerMessage, string raw)
		{
			strUser = user;
			strCommand = command;
			strParameters = parameters;
			intdescriptionBeginsAtIndex = descriptionBeginsAtIndex;
			blIsServerMessage = isServerMessage;
			strRaw = raw;
		}

		internal OnDataEventArgs(string user, string command, string[] parameters, bool isServerMessage, string raw) : this(user, command, parameters, -1, isServerMessage, raw)
		{
		}

		internal string User
		{
			get { return strUser; }
		}

		internal string Command
		{
			get { return strCommand; }
		}
		
		internal int descriptionBeginsAtIndex
		{
			get { return intdescriptionBeginsAtIndex; }
		}

		internal string[] Parameters
		{
			get { return strParameters; }
		}
		
		internal string description
		{
			get {
				string[] ret = new string[strParameters.Length - intdescriptionBeginsAtIndex];
				for (int x = intdescriptionBeginsAtIndex ; x < strParameters.Length ; x++)
					ret[x - intdescriptionBeginsAtIndex] = strParameters[x];
				
				return string.Join(" ", ret);
			}
		}
		
		internal bool IsServerMessage
		{
			get { return blIsServerMessage; }
		}
		
		internal string Raw
		{
			get { return strRaw; }
		}
	}
	#endregion
	#region Delegates
	internal delegate void OnDataEventHandler(object sender, OnDataEventArgs e);
	#endregion
	internal class ServerConnection
	{
		#region Private Variables
		private Socket socket;

		private DateTime dtLastActive = DateTime.Now;
		private Connection objOwner;
		#endregion
		#region Events
		internal event EventHandler OnConnected;
		internal event EventHandler OnConnectFailed;
		internal event EventHandler OnDisconnected;
		internal event OnDataEventHandler OnDataReceived;
		#endregion
		#region Connect
		internal void Connect(string host, int port)
		{
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			try
			{
				socket.BeginConnect(new IPEndPoint(Dns.GetHostEntry(host).AddressList[0], port), new AsyncCallback(connectCallback), socket);
			}
			catch
			{
				if (OnConnectFailed!=null)
					OnConnectFailed(this, new EventArgs());
			}
		}

		private void connectCallback(IAsyncResult ar)
		{
            try
            {
                socket.EndConnect(ar);
            }
            catch
            {
                if (OnConnectFailed != null)
                    OnConnectFailed(this, new EventArgs());
            }

			if (!socket.Connected)
			{
				if (OnConnectFailed!=null)
					OnConnectFailed(this, new EventArgs());
				return;
			}
			
			if (OnConnected!=null)
				OnConnected(this, null);
			
			IRCStateObject stateObject = new IRCStateObject();
			socket.BeginReceive(stateObject.buffer, 0, IRCStateObject.bufferSize, SocketFlags.None, new AsyncCallback(recieveCallback), stateObject);
		}
		#endregion
		#region recieveCallback
		private void recieveCallback(IAsyncResult ar)
		{
			if (socket.Connected)
			{
				int read = 0;
				try
				{
					read = socket.EndReceive(ar);
				}
				catch
				{
					Disconnect();
					if (OnDisconnected!=null)
						OnDisconnected(this, new EventArgs());
				}

				IRCStateObject stateObject = (IRCStateObject) ar.AsyncState;

				if (read>0||stateObject.lastBuffer!="")
				{
					string strData = stateObject.lastBuffer + Encoding.Default.GetString(stateObject.buffer, 0, read);

					strData = strData.Replace("\r\n", "\n").Replace("\r", "\n");

                    while (strData.Length > 0 && strData[0] == '\n')
                        strData = strData.Substring(1);

					//try
					//{
						stateObject.lastBuffer = onData(strData);
					//}
					//catch (Exception e)
					//{
					//	Console.WriteLine("Data could not be parsed:");
					//	Console.WriteLine(e.Message);
					//	Console.WriteLine(e.Source);
					//	Console.WriteLine(e.StackTrace);
					//}
				}

				if (socket.Connected)
				{
					try { socket.BeginReceive(stateObject.buffer, 0, IRCStateObject.bufferSize, SocketFlags.None, new AsyncCallback(recieveCallback), stateObject); }
					catch
					{
						if (OnDisconnected!=null)
							OnDisconnected(this, new EventArgs());
					}
				}
				else
				{
					if (OnDisconnected!=null)
						OnDisconnected(this, new EventArgs());
				}
			}
			else
			{
				if (OnDisconnected!=null)
					OnDisconnected(this, new EventArgs());
			}
		}
		#endregion
		#region onData
		private string onData(string strData)
		{
			string[] strLines = strData.Split(new char[] {'\n'});

			string strCommand;
			string[] strParams;
			string strParam = null;

			for (int x=0;x<strLines.Length - 1;x++)
			{
#if DEBUG
				System.Diagnostics.Debug.WriteLine(strLines[x]);
#endif
#if DEBUGCONSOLE
				Console.WriteLine(strLines[x]);
#endif

				//try
				{
					string strUser;
					bool blIsServerMessage;
	
	                if (strLines[x].Length > 0)
	                {
	                    if (strLines[x].Substring(0, 1) == ":")
	                    {
	                        strLines[x] = strLines[x].Substring(1, strLines[x].Length - 1);
	
							string[] strSegments = strLines[x].Split(new char[] { ' ' });
							// We probably dealing with a server code message
							if (strSegments.Length > 3 && strSegments[1].Length == 3 && strSegments[1][0] >= '0' && strSegments[1][0] <= '9')
							{
								strUser = strSegments[0];
								strCommand = strSegments[1];
								// [2] is just our own nick
								
								int descriptionBeginsAtIndex = -1; 
								strParams = new string[strSegments.Length - 3];
								for (int i = 3 ; i < strSegments.Length; i++)
								{
									if (strSegments[i].Length > 1 && strSegments[i][0] == ':' && descriptionBeginsAtIndex == -1)
									{
										strParams[i - 3] = strSegments[i].Substring(1);
										descriptionBeginsAtIndex = i - 3;
									}
									else
										strParams[i - 3] = strSegments[i];
								}
								
								if (OnDataReceived != null)
									OnDataReceived(this, new OnDataEventArgs(strUser, strCommand, strParams, descriptionBeginsAtIndex, true, strLines[x]));
							}
							else
							{
								// This following code is old parsing code. It holds back a lot of properties from the parser, but it works.
								
								if (strLines[x].IndexOf(" :") > -1)
								{
									strParam = strLines[x].Substring(strLines[x].IndexOf(" :") + 2, strLines[x].Length - (strLines[x].IndexOf(" :") + 2));
									strLines[x] = strLines[x].Substring(0, strLines[x].IndexOf(" :") + 1).Trim();
								}
								
								string[] strParts = strLines[x].Split(new char[] { ' ' });
								
								strUser = strParts[0];
								strCommand = strParts[1];
								blIsServerMessage = strParts[1][0] >= '0' && strParts[1][0] <= '9';
	
								int size = strParts.Length - 2;
								if (strParam != null)
									size++;
								strParams = new string[size];
								
								for (int y = 2; y < strParts.Length; y++)
									strParams[y - 2] = strParts[y];
								
								if (strParam != null)
									strParams[strParams.Length - 1] = strParam;
								
								if (OnDataReceived != null)
									OnDataReceived(this, new OnDataEventArgs(strUser, strCommand, strParams, blIsServerMessage, strLines[x]));
							}
	                    }
	                    else
	                    {
	                        if (strLines[x].IndexOf(":") > 0)
	                        {
	                            strParam = strLines[x].Substring(strLines[x].IndexOf(":") + 1, strLines[x].Length - (strLines[x].IndexOf(":") + 1));
	                            strLines[x] = strLines[x].Substring(0, strLines[x].IndexOf(":"));
	                        }
	
	                        string[] strParts = strLines[x].Split(new char[] { ' ' });
	
	                        strCommand = strParts[0];
	                        int size = strParts.Length - 2;
	                        if (strParam != null)
	                            size++;
	                        strParams = new string[size];
	
	                        for (int y = 1; y < strParts.Length; y++)
	                            strParams[y - 1] = strParts[y];
	
	                        if (strParam != null)
	                            strParams[strParams.Length - 1] = strParam;
	
	                        if (OnDataReceived != null)
	                            OnDataReceived(this, new OnDataEventArgs(null, strCommand, strParams, false, strLines[x]));
	                    }
	                }
				}
				//catch (Exception e)
				//{
#if DEBUG
					//System.Diagnostics.Debug.WriteLine("  ^^^^ Could not parse line:");
					//System.Diagnostics.Debug.WriteLine("       Error: {0}", e.Message);
#endif
#if DEBUGCONSOLE
					//Console.WriteLine("  ^^^^ Could not parse line:");
					//Console.WriteLine("       Error: {0}", e.Message);
#endif
				//}
			}

            return strLines[strLines.Length - 1];
		}
		#endregion
		#region SendData
		internal void SendData(string strData, bool blActive, bool blCloseAfterSend, params object[] objs)
		{
			if (blActive)
				dtLastActive = DateTime.Now;

            string strFormattedData = string.Format(strData, objs);

#if DEBUG
			System.Diagnostics.Debug.WriteLine(strFormattedData);
#endif
#if DEBUGCONSOLE
			Console.WriteLine(strFormattedData);
#endif

			byte[] data = Encoding.Default.GetBytes(string.Format("{0}{1}", strFormattedData, Environment.NewLine));

			if (socket != null && socket.Connected)
				socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(sendCallback), blCloseAfterSend);			
		}
		
		internal void SendData(string strData, params object[] objs)
		{
			SendData(strData, true, false, objs);
		}
		
		internal void SendData(string strData, bool blActive, params object[] objs)
		{
			SendData(strData, blActive, false, objs);
		}

		private void sendCallback(IAsyncResult ar)
		{
			bool blCloseAfterSend = (bool) ar.AsyncState;
            try
            {
                socket.EndSend(ar);
            }
            catch
            {
                if (OnDisconnected != null)
                    OnDisconnected(this, new EventArgs());
            }
			if (socket != null && blCloseAfterSend && socket.Connected)
				socket.Close();
		}
		#endregion
		#region Properties
		internal bool IsConnected
		{
			get { return socket!=null&&socket.Connected; }
		}

		internal byte[] RemoteIP
		{
			get 
			{
				if (socket.Connected)
					return ((IPEndPoint) socket.RemoteEndPoint).Address.GetAddressBytes();
				return null;
			}
		}

		internal int RemotePort
		{
			get
			{
				if (socket.Connected)
					return ((IPEndPoint) socket.RemoteEndPoint).Port;
				return 0;
			}
		}

		internal int LocalPort
		{
			get
			{
				if (socket.Connected)
					return ((IPEndPoint) socket.LocalEndPoint).Port;
				return 0;
			}
		}

		internal DateTime LastActive
		{
			get { return dtLastActive; }
		}
		#endregion
		#region Disconnect
		internal void Disconnect()
		{
			if (socket.Connected)
			{
				socket.Shutdown(SocketShutdown.Both);
				socket.Close();
			}
		}
		#endregion

		internal Connection Owner
		{
			get { return objOwner; }
			set { objOwner = value; }
		}
	}
}
