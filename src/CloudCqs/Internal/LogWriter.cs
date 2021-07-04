using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CloudCqs.Internal
{
    public class LogWriter
    {
        public LogContext LogContext { get; private set; }

        public LogWriter(LogContext logContext)
        {
            LogContext = logContext;
        }

        public async Task Trace(string description, Func<Task> func, Func<object?> getRequest, Func<object?> getResponse)
        {
            var traceName = $"{GetType()}:{description}";
            var start = DateTime.UtcNow;

            try
            {
                LogContext.Logger.LogInformation($"Start {traceName} with {getRequest()}");
                await LogContext.Tracer(traceName, async () => await func());
                LogContext.Logger.LogInformation($"End {traceName} in {DateTime.UtcNow - start} with {getResponse()}");
            }
            catch (ValidationException)
            {
                LogContext.Logger.LogInformation($"End {traceName} in {DateTime.UtcNow - start} with validation errors.");
                throw;
            }
            catch (Exception e)
            {
                LogContext.Logger.LogError($"Error {traceName} with {e.Message}");
                throw;
            }
        }
    }
}
