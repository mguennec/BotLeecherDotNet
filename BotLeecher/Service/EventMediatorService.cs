using BotLeecher.Model;
using BotLeecherWPF.Event;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher.Service
{
    [Export]
    public class EventMediatorService {

        public event EventHandler<MessageEventArgs> MessageEvent;
        public event EventHandler<PackEventArgs> PackEvent;
        public event EventHandler<UserListEventArgs> UserListEvent;
        public event EventHandler<TransferStatusEventArgs> TransferStatusEvent;

        protected virtual void OnMessageEvent(MessageEventArgs args)
        {
            EventHandler<MessageEventArgs> handler = MessageEvent;
            if (handler != null)
            {
                handler(this, args);
            }
        }
        protected virtual void OnUserListEvent(UserListEventArgs args)
        {
            EventHandler<UserListEventArgs> handler = UserListEvent;
            if (handler != null)
            {
                handler(this, args);
            }
        }
        protected virtual void OnPackEvent(PackEventArgs args)
        {
            EventHandler<PackEventArgs> handler = PackEvent;
            if (handler != null)
            {
                handler(this, args);
            }
        }
        protected virtual void OnTransferStatusEvent(TransferStatusEventArgs args)
        {
            EventHandler<TransferStatusEventArgs> handler = TransferStatusEvent;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public void SendMessage(string message, MessageType type)
        {
            OnMessageEvent(new MessageEventArgs(message, type));
        }

        public void SendPack(string botName, IList<Pack> packList)
        {
            OnPackEvent(new PackEventArgs(botName, packList));
        }

        public void SendUserList(IList<string> users)
        {
            OnUserListEvent(new UserListEventArgs(users));
        }

        public void SendTransferStatus(string botName, string fileName, int completion)
        {
            OnTransferStatusEvent(new TransferStatusEventArgs(botName, fileName, completion));
        }

    }

    public enum MessageType
    {
        DOWNLOAD, INFO, ERROR, REQUEST
    }
}
