using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.EmployeeManagement.DAL.Logger
{
    public class LoggerManager : ILoggerManager
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        public void LogDebug(string message) => logger.Debug(message);

        public void LogError(string message) => logger.Error(message);
        public void LogWarn(string message) => logger.Warn(message);
        public void LogInfo(string message) => logger.Info(message);

    }
}
