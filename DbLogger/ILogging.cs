using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace financing_api.DbLogger
{
    public interface ILogging
    {
        void LogTrace(string message);
        void LogException(Exception? exception);
        void LogDataExchange(string messageSource, string messageTarget, string methodCall, string messagePayload);
    }
}