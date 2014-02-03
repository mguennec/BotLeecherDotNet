// Type: NetIrc2.IrcReplyCode
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

namespace BotLeecher.NetIrc
{
    /// <summary>
    /// Many of the common IRC error and reply codes.
    /// 
    /// </summary>
    public enum IrcReplyCode
    {
        NoSuchNickname = 401,
        NoSuchServer = 402,
        NoSuchChannel = 403,
        CannotSendToChannel = 404,
        TooManyChannels = 405,
        WasNoSuchNickname = 406,
        TooManyTargets = 407,
        NoOriginSpecified = 409,
        NoRecipientGiven = 411,
        NoTextToSend = 412,
        NoTopLevelDomainSpecified = 413,
        WildcardInTopLevelDomain = 414,
        UnknownCommand = 421,
        MissingMOTD = 422,
        NoAdminInfoAvailable = 423,
        FileError = 424,
        NoNicknameGiven = 431,
        ErroneousNickname = 432,
        NicknameInUse = 433,
        NicknameCollision = 436,
        ResourceUnavailable = 437,
        UserNotInChannel = 441,
        NotInChannel = 442,
        UserAlreadyInChannel = 443,
        UserNotLoggedIn = 444,
        SummonCommandDisabled = 445,
        UsersCommandDisabled = 446,
        HaveNotRegistered = 451,
        NotEnoughParameters = 461,
        AlreadyRegistered = 462,
        UnpriviledgedHost = 463,
        IncorrectPassword = 464,
        BannedFromServer = 465,
        ChannelKeyAlreadySet = 467,
        ChannelIsFull = 471,
        UnknownModeCharacter = 472,
        InviteOnlyChannel = 473,
        BannedFromChannel = 474,
        BadChannelKey = 475,
        BadChannelName = 479,
        NotIRCOperator = 481,
        NotChannelOperator = 482,
        CannotKillServer = 483,
        HostCannotUseOperCommand = 491,
        UnknownModeFlag = 501,
    }
}
