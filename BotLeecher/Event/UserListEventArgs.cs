using BotLeecher.Model;
using BotLeecher.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecherWPF.Event
{
    public class UserListEventArgs
    {

        public IList<string> Users { get; private set; }

        public UserListEventArgs(IList<string> users)
        {
            this.Users = new List<string>(users);
        }
    }
}
