using BotLeecher.NetIrc;
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
        BotLeecher getBotLeecher(IrcString user, IrcConnection connection);
    }
}
