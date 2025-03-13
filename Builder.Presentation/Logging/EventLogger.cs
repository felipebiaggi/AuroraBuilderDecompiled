using Builder.Core.Events;
using Builder.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Logging
{
    public class EventLogger : ILogger
    {
        private readonly IEventAggregator _eventAggregator;

        public EventLogger(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void Debug(string message, params object[] args)
        {
            _eventAggregator.Send(new EventLog(DateTime.Now.ToString(), Log.Debug, string.Format(message, args)));
        }

        public void Info(string message, params object[] args)
        {
            _eventAggregator.Send(new EventLog(DateTime.Now.ToString(), Log.Info, string.Format(message, args)));
        }

        public void Warning(string message, params object[] args)
        {
            _eventAggregator.Send(new EventLog(DateTime.Now.ToString(), Log.Warning, string.Format(message, args)));
        }

        public void Exception(Exception ex)
        {
            _eventAggregator.Send(new EventLog(DateTime.Now.ToString(), Log.Exception, ex.Message));
        }
    }
}
