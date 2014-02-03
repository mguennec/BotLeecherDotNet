// Type: NetIrc2.Parsing.IrcStatementParseResult
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

namespace BotLeecher.NetIrc.Parsing
{
    /// <summary>
    /// The result of attempting to parse an IRC statement.
    /// 
    /// </summary>
    public enum IrcStatementParseResult
    {
        OK,
        NothingToParse,
        InvalidStatement,
        StatementTooLong,
        Disconnected,
        TimedOut,
    }
}
