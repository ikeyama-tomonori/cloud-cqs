using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CloudCqs.Internal
{
    public abstract class LogWriter
    {
        private CloudCqsOption Option { get; }
        private ILogger Logger { get; }

        protected LogWriter(CloudCqsOption option)
        {
            Option = option;

            if (Option.LoggerFactory != null)
            {
                Logger = Option.LoggerFactory.CreateLogger(GetType());
            }
            else
            {
                using var factory = LoggerFactory.Create(builder => builder.AddConsole());
                Logger = factory.CreateLogger(GetType());
            }
        }

        public async Task<object> Trace(string description, object request, Func<object, Task<object>> handler)
        {
            var start = DateTime.UtcNow;
            try
            {
                var response = Option.Interceptor != null
                    ? await Option.Interceptor(
                        $"{GetType()}:[{description}]",
                        request,
                        req => handler(req))
                    : await handler(request);
                Logger.LogInformation(
                    "[{Name}] completed in {Duration} with {Status}. Request = {Request}, Response = {Response}",
                    description,
                    DateTime.UtcNow - start,
                    "success",
                    request,
                    response);
                return response;
            }
            catch (ValidationException)
            {
                Logger.LogWarning(
                    "[{Name}] completed in {Duration} with {Status}. Request = {Request}",
                    description,
                    DateTime.UtcNow - start,
                    "validation error",
                    request);
                throw;
            }
            catch (Exception e)
            {
                Logger.LogError(
                    e,
                    "[{Name}] completed in {Duration} with {Status}. Request = {Request}, Exception = {Exception}",
                    description,
                    DateTime.UtcNow - start,
                    "exception",
                    request,
                    e.Message);
                throw;
            }
        }
    }
}
