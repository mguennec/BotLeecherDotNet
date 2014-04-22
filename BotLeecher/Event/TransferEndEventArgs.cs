using BotLeecher.Model;
using BotLeecher.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecherWPF.Event
{
    public class TransferEndEventArgs
    {

        public string BotName { get; private set; }
        public string FileName { get; private set; }
        public bool Completed { get; private set; }


        public TransferEndEventArgs(string botName, string fileName, bool isComplete)
        {
            this.BotName = botName;
            this.FileName = fileName;
            this.Completed = isComplete;
        }
    }
}
