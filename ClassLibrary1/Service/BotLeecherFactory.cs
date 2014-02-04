using BotLeecher.NetIrc;
using ircsharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher.Service
{
    public interface BotLeecherFactory
    {

        /**
         * Creates a botleecher
         *
         * @param user
         * @return
         */
        BotLeecher GetBotLeecher(User user, IrcConnection connection);
    }
}
