using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Common.Logging
{
    public class NLogLogger : ILogger
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Debug(string message)
        {
            Logger.Debug(message);
        }

        public void Info(string message)
        {
            Logger.Info(message);
        }

        public void Warn(string message)
        {
            Logger.Warn(message);
        }

        public void Error(string message)
        {
            Logger.Error(message);
        }

        public void Error(string message, Exception exception)
        {
            Logger.Error(message + Environment.NewLine + exception);
        }
    }
}
