using System;

namespace Common.Interfaces
{
    public interface ILog
    {
        void Debug(string message, params object[] args);
        void Debug(Exception ex, string message, params object[] args);

        void Info(string message, params object[] args);
        void Info(Exception ex, string message, params object[] args);

        void Warning(string message, params object[] args);
        void Warning(Exception ex, string message, params object[] args);

        void Error(string message, params object[] args);
        void Error(Exception ex, string message, params object[] args);
    }
}