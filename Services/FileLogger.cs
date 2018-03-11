using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Polyclinic.Services
{
    public class FileLogger : ILogger
    {
        private string _path;
        private string _categoryname;
        private LogLevel _logLevel;

        private static object _lock = new object();

        public FileLogger(string path, LogLevel logLevel, string categoryname)
        {
            _path = path;
            _categoryname = categoryname;
            _logLevel = logLevel;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _logLevel; 
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                lock (_lock)
                {
                    File.AppendAllText(_path, logLevel + Environment.NewLine 
                                              + _categoryname + Environment.NewLine 
                                              + formatter(state, exception) + Environment.NewLine);
                }
            }
        }
    }
}
