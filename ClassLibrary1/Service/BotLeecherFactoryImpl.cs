using BotLeecher.NetIrc;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher.Service
{
    [Export(typeof(BotLeecherFactory))]
    public class BotLeecherFactoryImpl : BotLeecherFactory {
    private readonly Settings settings;
    private readonly PackListReader packListReader;

    [ImportingConstructor]
    public BotLeecherFactoryImpl(Settings settings, PackListReader packListReader) {
        this.settings = settings;
        this.packListReader = packListReader;
    }

    public BotLeecher getBotLeecher(IrcString user, IrcConnection connection) {
        return new BotLeecher(user, connection, settings, packListReader);
    }

}
}
