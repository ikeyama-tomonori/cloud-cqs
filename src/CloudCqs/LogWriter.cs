using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using System;
using System.Threading.Tasks;

namespace CloudCqs
{
    public abstract class LogWriter
    {
        private CloudCqsOptions Option { get; }
        private ILogger Logger { get; }

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
                var start = DateTime.UtcNow;
                try
                {
                    var response = await handler(innerRequest);
                    Logger.LogInformation(
                        "[{Name}] completed in {Duration}ms. Request = {Request}, Response = {Response}",
                        description,
                        (DateTime.UtcNow - start).TotalMilliseconds,
                        request,
                        response);
                    return response;
                }
                catch (StatusCodeException exception)
                {
                    Logger.LogWarning(
                        exception,
                        "[{Name}] terminated in {Duration}ms. Request = {Request}",
                        description,
                        (DateTime.UtcNow - start).TotalMilliseconds,
                        request);
                    throw;
                }
                catch (Exception exception)
                {
                    Logger.LogError(
                        exception,
                        "[{Name}] terminated in {Duration}ms. Request = {Request}",
                        description,
                        (DateTime.UtcNow - start).TotalMilliseconds,
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
