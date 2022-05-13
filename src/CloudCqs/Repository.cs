using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CloudCqs;

public abstract class Repository<TRequest, TResponse> : LogWriter, IRepository<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
    private BuiltHandler? Handler { get; set; }

    protected Repository(CloudCqsOptions option) : base(option)
    {
    }

    public async Task<TResponse> Invoke(TRequest request)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var dataValidation = new Function("Validate request data by annotations",
            param =>
            {
                param.Validate();
                return Task.FromResult(param);
            });
        var functions = Handler?.Functions ?? Array.Empty<Function>();

        var response = await functions
            .Prepend(dataValidation)
            .Aggregate(
                Task.FromResult(request as object),
                async (acc, cur) => await Trace(
                    cur.Description,
                    await acc,
                    req => cur.Func(req)));

        stopwatch.Stop();
        Logger.LogInformation(
            "Executed [{Name}] ({Duration}ms) [Request={Request}, Response={Response}]",
            GetType().Name,
            stopwatch.ElapsedMilliseconds,
            request,
            response);
        if (response is TResponse res) return res;
        throw new TypeGuardException(typeof(TResponse), response);
    }

    protected void SetHandler(BuiltHandler handler)
    {
        Handler = handler;
    }
}
