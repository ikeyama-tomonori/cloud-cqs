using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CloudCqs.Test
{
    public static class Options
    {
        private static ILoggerFactory Logger => LoggerFactory.Create(configure =>
        {
            configure.AddConsole();
            configure.AddFilter(_ => true);
        });
        public static CloudCqsOptions Instance => new(Logger);
    }
}
