using Common.Interfaces;
using System;
using AnyExtend;

namespace Host
{
    class ConsoleLog : ILog
    {
        private void WriteLine(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
        }

        private string LogTime() => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff");

        public void Debug(string message, params object[] args)
        {
            WriteLine(LogTime() + " (debug)：" + StringExt.Format(message, args), ConsoleColor.Gray);
        }

        public void Debug(Exception ex, string message, params object[] args)
        {
            Debug(message, args);
            WriteLine("----以下是调试信息-----");
            WriteLine(ex.ToString());
        }

        public void Error(string message, params object[] args)
        {
            WriteLine(LogTime() + " (error)：" + StringExt.Format(message, args), ConsoleColor.DarkRed);
        }

        public void Error(Exception ex, string message, params object[] args)
        {
            Error(message, args);
            WriteLine("----以下是异常信息-----");
            WriteLine(ex.ToString());
        }

        public void Info(string message, params object[] args)
        {
            WriteLine(LogTime() + " (info)：" + StringExt.Format(message, args), ConsoleColor.Green);
        }

        public void Info(Exception ex, string message, params object[] args)
        {
            Info(message, args);
            WriteLine("----以下是输出信息-----");
            WriteLine(ex.ToString());
        }

        public void Warning(string message, params object[] args)
        {
            WriteLine(LogTime() + " (warning)：" + StringExt.Format(message, args), ConsoleColor.Yellow);
        }

        public void Warning(Exception ex, string message, params object[] args)
        {
            Warning(message, args);
            WriteLine("----以下是警告信息-----");
            WriteLine(ex.ToString());
        }
    }
}
