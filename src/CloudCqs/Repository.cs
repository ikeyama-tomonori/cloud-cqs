namespace CloudCqs;

using System.Diagnostics;

public abstract class Repository<TRequest, TResponse> : IRepository<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
    private readonly CloudCqsOptions options;
    private TRequest? request;
    private CancellationToken cancellationToken;
    private Handler<TResponse, TResponse>? handler;

    protected Repository(CloudCqsOptions options)
    {
        this.options = options;
    }

    public async Task<TResponse> Invoke(
        TRequest request,
        CancellationToken cancellationToken = default
    )
    {
        this.request = request;
        this.cancellationToken = cancellationToken;

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            if (this.handler == null)
            {
                throw new NullGuardException(nameof(this.handler));
            }

            var response = await this.handler.Functions.Aggregate(
                Task.FromResult(new object()),
                async (acc, cur) =>
                {
                    var param = await acc;
                    var thisStopwatch = new Stopwatch();

                    thisStopwatch.Start();
                    try
                    {
                        var result = await cur.Func(param, cancellationToken);
                        thisStopwatch.Stop();
                        this.options.FunctionExecuted(
                            (
                                RepositoryType: this.GetType(),
                                Description: cur.Description,
                                Param: param,
                                Result: result,
                                TimeSpan: thisStopwatch.Elapsed
                            )
                        );

                        return result;
                    }
                    catch (Exception exception)
                    {
                        thisStopwatch.Stop();
                        this.options.FunctionTerminated(
                            (
                                RepositoryType: this.GetType(),
                                Description: cur.Description,
                                Param: param,
                                Exception: exception,
                                TimeSpan: thisStopwatch.Elapsed
                            )
                        );
                        throw;
                    }
                }
            );

            stopwatch.Stop();
            this.options.RepositoryExecuted(
                (
                    RepositoryType: this.GetType(),
                    Request: request,
                    Response: response,
                    TimeSpan: stopwatch.Elapsed
                )
            );

            if (response is TResponse res)
            {
                return res;
            }
            throw new TypeGuardException(typeof(TResponse), response);
        }
        catch (Exception exception)
        {
            this.options.RepositoryTerminated(
                (
                    RepositoryType: this.GetType(),
                    Request: request,
                    Exception: exception,
                    TimeSpan: stopwatch.Elapsed
                )
            );
            throw;
        }
    }

    protected TRequest UseRequest() => this.request ?? throw new NullGuardException("Request");

    protected CancellationToken UseCancellationToken() => this.cancellationToken;

    protected void SetHandler(Handler<TResponse, TResponse> handler)
    {
        this.handler = handler;
    }

    protected class Handler : Handler<object, TResponse>
    {
        public Handler() : base(Array.Empty<Function>()) { }
    }
}
