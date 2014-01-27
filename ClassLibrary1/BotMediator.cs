using IrcDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher
{
    public class BotMediator
    {
        private IrcClient Client;
        

        public BotMediator() {
            Client = new IrcClient();
        }

        public void Disconnect() 
        {

        }

        public void Connect()
        {

        }
    }
}
