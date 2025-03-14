using Builder.Core.Logging;
using System;
using System.IO;

namespace Builder.Presentation.Logging
{
    public class FileLogger : ILogger
    {
        private readonly string _directory;

        private readonly string _infoFilename;

        private readonly string _errorsFilename;

        public FileLogger(string directory)
        {
            _directory = directory;
            _infoFilename = "info." + DateTime.Today.ToString("yyyyMMdd") + ".log";
            _errorsFilename = "errors." + DateTime.Today.ToString("yyyyMMdd") + ".log";
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidFileNameChars)
            {
                _infoFilename = _infoFilename.Replace(c.ToString(), "");
            }
            invalidFileNameChars = Path.GetInvalidFileNameChars();
            foreach (char c2 in invalidFileNameChars)
            {
                _errorsFilename = _errorsFilename.Replace(c2.ToString(), "");
            }
            Info("======================================== New Session ========================================");
            Warning("======================================== New Session ========================================");
        }

        public void Debug(string message, params object[] args)
        {
        }

        public void Info(string message, params object[] args)
        {
            try
            {
                string text = message;
                if (args != null)
                {
                    text = string.Format(message, args);
                }
                File.AppendAllText(Path.Combine(_directory, _infoFilename), GeneratePrefix(Log.Info) + text + Environment.NewLine);
            }
            catch (Exception)
            {
            }
        }

        public void Warning(string message, params object[] args)
        {
            try
            {
                string text = message;
                if (args != null)
                {
                    text = string.Format(message, args);
                }
                File.AppendAllText(Path.Combine(_directory, _errorsFilename), GeneratePrefix(Log.Warning) + text + Environment.NewLine);
            }
            catch (Exception)
            {
            }
        }

        public void Exception(Exception ex)
        {
            string text = string.Format("{0} {1}: {2}", ex.GetType().Name, (ex.InnerException != null) ? "has inner exception" : "with no inner exception", ex.Message);
            text = text + Environment.NewLine + "Source: " + ex.Source;
            text = text + Environment.NewLine + "Trace: " + ex.StackTrace;
            if (ex.InnerException != null)
            {
                text = text + Environment.NewLine + $"Inner Exception: {ex.InnerException}";
            }
            WriteLog(Log.Exception, text);
        }

        private static string GeneratePrefix(Log log)
        {
            return DateTime.UtcNow.ToString("u") + " | " + log.ToString().ToUpper() + " | ";
        }

        private void WriteLog(Log log, string data)
        {
            try
            {
                string path = _errorsFilename;
                if (log == Log.Debug)
                {
                    path = "debug.log";
                }
                File.AppendAllText(Path.Combine(_directory, path), GeneratePrefix(log) + data + Environment.NewLine);
            }
            catch (Exception)
            {
            }
        }
    }
}
