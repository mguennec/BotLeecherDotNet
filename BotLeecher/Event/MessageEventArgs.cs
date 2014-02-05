using BotLeecher.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecherWPF.Event
{
    public class MessageEventArgs
    {
        /// <summary>
        /// The message.
        /// 
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// The message type.
        /// 
        /// </summary>
        public MessageType Type { get; private set; }

        public MessageEventArgs(string msg, MessageType type)
        {
            this.Message = msg;
            this.Type = type;
        }
    }
}
