using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CloudCqs
{
    public record CloudCqsOption(
        ILoggerFactory? LoggerFactory = null,
        Func<string, object, Func<object, Task<object>>, Task<object>>? Interceptor = null);
}
