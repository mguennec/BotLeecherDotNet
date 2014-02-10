using ircsharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotLeecher.Tools
{
    public class EventWaiter<T>
    {

        private AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        private EventInfo _event = null;
        private object _eventContainer = null;

        public EventWaiter(object eventContainer, string eventName)
        {
            _eventContainer = eventContainer;
            _event = eventContainer.GetType().GetEvent(eventName);
        }

        public void WaitForEvent(TimeSpan timeout)
        {
            EventHandler<T> eventHandler = new EventHandler<T>((sender, args) => { _autoResetEvent.Set(); });
            _event.AddEventHandler(_eventContainer, eventHandler);
            _autoResetEvent.WaitOne(timeout);
            _event.RemoveEventHandler(_eventContainer, eventHandler);
        }

    }
    public class IrcEventWaiter
    {

        private AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        private EventInfo _event = null;
        private object _eventContainer = null;

        public IrcEventWaiter(object eventContainer, string eventName)
        {
            _eventContainer = eventContainer;
            _event = eventContainer.GetType().GetEvent(eventName);
        }

        public void WaitForEvent(TimeSpan timeout)
        {
            IRCEventHandler eventHandler = new IRCEventHandler((sender, args) => { _autoResetEvent.Set(); });
            _event.AddEventHandler(_eventContainer, eventHandler);
            _autoResetEvent.WaitOne(timeout);
            _event.RemoveEventHandler(_eventContainer, eventHandler);
        }

    }
}
