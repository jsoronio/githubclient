using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GitHubClient.Services.Interface
{
    public interface ILog
    {
        void Information(string message);
        void Warning(string message);
        void Debug(string message);
        void Error(string message);
        void Error(Exception exception, string message);
        void Error(WebException webException, string message);
    }
}
