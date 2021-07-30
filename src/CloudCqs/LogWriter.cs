using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CloudCqs
{
    public abstract class LogWriter
    {
        private CloudCqsOptions Option { get; }
        protected ILogger Logger { get; }

        protected LogWriter(CloudCqsOptions option)
        {
            Option = option;

            if (Option.LoggerFactory != null)
            {
                Logger = Option.LoggerFactory.CreateLogger(GetType());
            }
            else
            {
                Logger = NullLogger.Instance;
            }
        }

        public async Task<object> Trace(string description, object request, Func<object, Task<object>> handler)
        {
            async Task<object> inner(object innerRequest)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                try
                {
                    var response = await handler(innerRequest);
                    stopwatch.Stop();
                    Logger.LogDebug(
                        "[{Name}] completed in {Duration}ms. Request = {Request}, Response = {Response}",
                        description,
                        stopwatch.ElapsedMilliseconds,
                        request,
                        response);
                    return response;
                }
                catch (StatusCodeException exception)
                {
                    stopwatch.Stop();
                    Logger.LogWarning(
                        exception,
                        "[{Name}] terminated in {Duration}ms. Request = {Request}",
                        description,
                        stopwatch.ElapsedMilliseconds,
                        request);
                    throw;
                }
                catch (Exception exception)
                {
                    stopwatch.Stop();
                    Logger.LogError(
                        exception,
                        "[{Name}] terminated in {Duration}ms. Request = {Request}",
                        description,
                        stopwatch.ElapsedMilliseconds,
                        request);
                    throw;
                }
            }

            if (Option.Interceptor == null)
            {
                return await inner(request);
            }
            var response = await Option.Interceptor((
                    $"{GetType()}:[{description}]",
                    request,
                    innerRequest => inner(innerRequest)));
            return response;
        }
    }
}
