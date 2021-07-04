using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CloudCqs
{
    public class LogContext
    {
        public ILogger Logger { get; init; }

        public Func<string, Func<Task>, Task> Tracer { get; init; }

        public LogContext(ILogger? logger = null, Func<string, Func<Task>, Task>? tracer = null)
        {
            if (logger == null)
            {
                using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
                Logger = loggerFactory.CreateLogger<LogContext>();
            }
            else
            {
                Logger = logger;
            }

            Func<string, Func<Task>, Task> defaultTracer = async (_, func) => await func();
            Tracer = tracer ?? defaultTracer;
        }
    }
}
