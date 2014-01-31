using BotLeecher.Model;
using BotLeecher.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecherWPF.Event
{
    public class TransferStatusEventArgs
    {

        public string BotName { get; private set; }
        public string FileName { get; private set; }
        public int Completion { get; private set; }

        public TransferStatusEventArgs(string botName, string fileName, int completion)
        {
            this.BotName = botName;
            this.FileName = fileName;
            this.Completion = completion;
        }
    }
}
