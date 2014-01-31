using BotLeecher.Model;
using BotLeecher.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecherWPF.Event
{
    public class PackEventArgs
    {
        public string BotName { get; private set; }

        public IList<Pack> PackList { get; private set; }

        public PackEventArgs(string botName, IList<Pack> packList)
        {
            this.BotName = botName;
            this.PackList = packList;
        }
    }
}
