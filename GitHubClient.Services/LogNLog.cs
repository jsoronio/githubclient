using GitHubClient.Services.Interface;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GitHubClient.Services
{
    public class LogNLog : ILog
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        public LogNLog()
        {
        }

        public void Information(string message)
        {
            logger.Info(message);
        }

        public void Warning(string message)
        {
            logger.Warn(message);
        }

        public void Debug(string message)
        {
            logger.Debug(message);
        }

        public void Error(string message)
        {
            logger.Error(message);
        }

        public void Error(Exception exception, string message)
        {
            logger.Error(exception, message);
        }

        public void Error(WebException webException,string message)
        {
            logger.Error(webException, message);
        }
    }
}
