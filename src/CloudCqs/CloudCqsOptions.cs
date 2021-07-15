using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CloudCqs
{
    public record CloudCqsOptions(
        ILoggerFactory? LoggerFactory = null,
        Func<(string description, object request, Func<object, Task<object>>),
            Task<object>>? Interceptor = null);
}
