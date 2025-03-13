using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Builder.Core.Logging
{
    public static class Logger
    {
        private static readonly Dictionary<string, ILogger> Loggers = new Dictionary<string, ILogger>();

        private static bool _isEnabled = true;

        public static bool IsEnabled
        {
            private get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                Info(_isEnabled ? "Logger Enabled" : "Logger Disabled");
            }
        }

        public static void Initializing(object initObject)
        {
            Info($"initializing: [{initObject}]");
        }

        public static void Debug(string message, params object[] args)
        {
            if (!IsEnabled)
            {
                return;
            }
            foreach (ILogger value in Loggers.Values)
            {
                value.Debug(message, args);
            }
        }

        public static void Info(string message, params object[] args)
        {
            if (!IsEnabled)
            {
                return;
            }
            foreach (ILogger value in Loggers.Values)
            {
                value.Info(message, args);
            }
        }

        public static void Warning(string message, params object[] args)
        {
            if (!IsEnabled)
            {
                return;
            }
            foreach (ILogger value in Loggers.Values)
            {
                value.Warning(message, args);
            }
        }

        public static void Exception(Exception ex, [CallerMemberName] string callingMethodName = "")
        {
            if (!IsEnabled)
            {
                return;
            }
            foreach (ILogger value in Loggers.Values)
            {
                value.Warning("Exception in {0}", callingMethodName);
                value.Exception(ex);
            }
        }

        public static void RegisterLogger(ILogger logger)
        {
            string name = logger.GetType().Name;
            if (!Loggers.ContainsKey(name))
            {
                Loggers.Add(name, logger);
                Info("registered logger implementation [" + name + "] with logger");
            }
        }
    }
}
