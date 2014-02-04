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

using System;

namespace ircsharp
{
	#region Delegates
	/// <summary>
	/// Used with the ConnectFailed event.
	/// </summary>
	public delegate void ConnectFailedEventHandler(Connection sender, ConnectFailedEventArgs e);
	/// <summary>
	/// Used with the ServerMessage event.
	/// </summary>
	public delegate void ServerMessageEventHandler(Connection sender, ServerMessageEventArgs e);
	/// <summary>
	/// Used with the Connected, BeforeDisconnect, Disconnect, PingPong, MOTDUpdated, ChannelListUpdated and LocalIPUpdated events.
	/// </summary>
	public delegate void IRCEventHandler(Connection sender, EventArgs e);
	/// <summary>
	/// Used with the BeforeError and Error events.
	/// </summary>
	public delegate void ErrorEventHandler(Connection sender, ErrorEventArgs e);
	/// <summary>
	/// Used with the Join event.
	/// </summary>
	public delegate void JoinEventHandler(Channel sender, JoinEventArgs e);
	/// <summary>
	/// Used with the JoinComplete event.
	/// </summary>
	public delegate void JoinCompleteEventHandler(Connection sender, JoinCompleteEventArgs e);
	public delegate void PartCompleteEventHandler(Connection sender, PartCompleteEventArgs e);
	/// <summary>
	/// Used with the ModeChange event.
	/// </summary>
	public delegate void ModeChangeEventHandler(Channel sender, ModeChangeEventArgs e);
	/// <summary>
	/// Used with the BeforePart and Part events.
	/// </summary>
	public delegate void PartEventHandler(Channel sender, PartEventArgs e);
	/// <summary>
	/// Used with the BeforeQuit and Quit events.
	/// </summary>
	public delegate void QuitEventHandler(User sender, QuitEventArgs e);
	/// <summary>
	/// Used with the Invitation event.
	/// </summary>
	public delegate void InvitationEventHandler(Connection sender, InvitationEventArgs e);
	/// <summary>
	/// Used with the Message, Notice and Action events.
	/// </summary>
	public delegate void MessageEventHandler(MessageReciever sender, MessageEventArgs e);
	/// <summary>
	/// Used with the CtCpMessage event.
	/// </summary>
	public delegate void CtCpMessageEventHandler(MessageReciever sender, CtCpMessageEventArgs e);
	/// <summary>
	/// Used when receiving a CTCP VERSION request. This request is when other users need to know your client. This event is trigered when this information is needed by the control.
	/// </summary>
	/// <param name="sender">
	/// A <see cref="Connection"/> needing the client information.
	/// </param>
	/// <param name="e">
	/// A <see cref="EventArgs"/> with nothing.
	/// </param>
	/// <returns>
	/// A <see cref="System.String"/> containing the clients information. Name, version, build, platform, etc.
	/// </returns>
	public delegate string CtCpVersionReplyEventHandler(Connection sender, EventArgs e);
	/// <summary>
	/// Used with the CtCpPingReply event.
	/// </summary>
	public delegate void CtCpPingReplyEventHandler(User sender, CtCpPingReplyEventArgs e);
	/// <summary>
	/// Used with the CtCpVersionReply, CtCpFingerReply, CtCpTimeReply events.
	/// </summary>
	public delegate void CtCpReplyEventHandler(User sender, CtCpReplyEventArgs e);
	/// <summary>
	/// Used with the CtCpReply event.
	/// </summary>
	public delegate void CtCpCommandReplyEventHandler(User sender, CtCpCommandReplyEventArgs e);
	/// <summary>
	/// Used with the BeforeKick and Kick events.
	/// </summary>
	public delegate void KickEventHandler(Channel sender, KickEventArgs e);
	/// <summary>
	/// Used with the ServerNotice event.
	/// </summary>
	public delegate void ServerNoticeEventHandler(Connection sender, ServerNoticeEventArgs e);
	/// <summary>
	/// Used with the NickChange event.
	/// </summary>
	public delegate void NickChangeEventHandler(User sender, NickChangeEventArgs e);
    /// <summary>
	/// Used with the NickChange event.
	/// </summary>
	public delegate void BeforeNickChangeEventHandler(User sender, BeforeNickChangeEventArgs e);
    /// <summary>
    /// Used with the ModeChanged event.
    /// </summary>
    public delegate void UserModeEventHandler(Channel sender, UserModeEventArgs e);
    /// <summary>
    /// Used with the DCCChatRequest event.
    /// </summary>
    public delegate void DCCChatRequestEventHandler(User sender, DCCChatRequestEventArgs e);
    /// <summary>
    /// Used with the DCCTransferRequest event.
    /// </summary>
	public delegate void DCCTransferRequestEventHandler(User sender, DCCTransferRequestEventArgs e);
    /// <summary>
    /// Used with the ChannelListUpdateProgress event.
    /// </summary>
    public delegate void ChannelListUpdateProgressEventHandler(Connection sender, ChannelListUpdateProgress e);
	/// <summary>
	/// Used when if the library is unable to connect because the nick is in use.
	/// </summary>
	/// <param name="sender">
	/// The <see cref="Connection"/> that needs the need nick.
	/// </param>
	/// <param name="e">
	/// A <see cref="IRCEventHandler"/> dummy event handler.
	/// </param>
	/// <returns>
	/// A <see cref="System.String"/> new nick name to provide to the server. Return null if no nick is available; the client will then disconnect.
	/// </returns>
	public delegate string NickInUseEventHandler(Connection sender, EventArgs e);
	/// <summary>
	/// Occurs when a users information is update. Eg. nickchange.
	/// </summary>
	public delegate void UserInfoEventHandler(User sender, EventArgs e);
	public delegate void UnknownUserEmergedEventHandler(Connection sender, UnknownUserEmergedEventArgs e);
	public delegate void RawDataEventHandler(Connection sender, RawDataEventArgs e);
	public delegate void TopicChangedEventHandler(Channel sender, TopicChangedEventArgs e);
	#endregion
}
	