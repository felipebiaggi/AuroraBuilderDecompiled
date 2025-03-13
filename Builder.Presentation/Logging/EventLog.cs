using Builder.Core.Events;
using Builder.Core.Logging;

namespace Builder.Presentation.Logging
{
    public class EventLog : EventBase
    {
        public string Message { get; }

        public Log Level { get; }

        public string Timestamp { get; }

        public EventLog(string timestamp, Log level, string message)
        {
            Message = message;
            Level = level;
            Timestamp = timestamp;
        }
    }

}
