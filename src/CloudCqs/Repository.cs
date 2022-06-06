using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CloudCqs;

public abstract class Repository<TRequest, TResponse> : IRepository<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
    private BuiltHandler? Handler { get; set; }
    private readonly CloudCqsOptions _options;

    protected Repository(CloudCqsOptions options)
    {
        _options = options;
    }

    public async Task<TResponse> Invoke(TRequest request)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
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
                    async (acc, cur) =>
                    {
                        var thisRequest = await acc;
                        var thisStopwatch = new Stopwatch();
                        thisStopwatch.Start();
                        try
                        {
                            var thisResponse = await cur.Func(thisRequest);
                            thisStopwatch.Stop();
                            _options.FunctionExecuted((
                                repositoryType: GetType(),
                                description: cur.Description,
                                request: thisRequest,
                                response: thisResponse,
                                timeSpan: thisStopwatch.Elapsed));

                            return thisResponse;
                        }
                        catch (Exception exception)
                        {
                            thisStopwatch.Stop();
                            _options.FunctionTerminated((
                                repositoryType: GetType(),
                                description: cur.Description,
                                request: thisRequest,
                                exception,
                                timeSpan: thisStopwatch.Elapsed));
                            throw;
                        }
                    });

            stopwatch.Stop();
            _options.RepositoryExecuted((
                repositoryType: GetType(),
                request,
                response,
                timeSpan: stopwatch.Elapsed));

            if (response is TResponse res) return res;
            throw new TypeGuardException(typeof(TResponse), response);
        }
        catch (Exception exception)
        {
            _options.RepositoryTerminated((
                repositoryType: GetType(),
                request,
                exception,
                timeSpan: stopwatch.Elapsed));
            throw;
        }
    }

    protected void SetHandler(BuiltHandler handler)
    {
        Handler = handler;
    }
}
