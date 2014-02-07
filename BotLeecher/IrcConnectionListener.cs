using ircsharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher
{
    public interface IrcConnectionListener {
        void UserListLoaded(string channel, IList<User> users);

        void OnMessage(User user, string message);

        void Disconnected(bool reconnect = false);
    }
}
