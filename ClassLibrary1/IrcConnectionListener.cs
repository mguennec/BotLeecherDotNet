using BotLeecher.NetIrc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher
{
    public interface IrcConnectionListener {
        void UserListLoaded(String channel, IList<IrcString> users);

        void Disconnected();
    }
}
