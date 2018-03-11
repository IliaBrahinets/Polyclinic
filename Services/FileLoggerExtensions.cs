using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Polyclinic.Services
{
    public static class FileLoggerExtensions
    {
        public static ILoggerFactory AddFile(this ILoggerFactory loggerFactory, string path,LogLevel logLevel = LogLevel.Information)
        {
            loggerFactory.AddProvider(new FileLoggerProvider(path, logLevel));

            return loggerFactory;
        }
    }
}
