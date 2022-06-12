namespace CloudCqs;

public abstract class Repository<TRequest, TResponse> : IRepository<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
    private Handler<TResponse, TResponse>? _handler;
    private readonly CloudCqsOptions _options;

    private TRequest? _request;
    protected TRequest UseRequest() => _request ?? throw new NullGuardException("Request");

    private CancellationToken _cancellationToken;
    protected CancellationToken UseCancellationToken() => _cancellationToken;

    protected Repository(CloudCqsOptions options)
    {
        _options = options;
    }

    public async Task<TResponse> Invoke(TRequest request, CancellationToken cancellationToken = default)
    {
        _request = request;
        _cancellationToken = cancellationToken;

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            if (_handler == null)
            {
                throw new NullGuardException(nameof(_handler));
            }

            var response = await _handler
                .Functions
                .Aggregate(
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
                            _options.FunctionExecuted((
                                repositoryType: GetType(),
                                description: cur.Description,
                                param,
                                result,
                                timeSpan: thisStopwatch.Elapsed));

                            return result;
                        }
                        catch (Exception exception)
                        {
                            thisStopwatch.Stop();
                            _options.FunctionTerminated((
                                repositoryType: GetType(),
                                description: cur.Description,
                                param,
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

    protected void SetHandler(Handler<TResponse, TResponse> handler)
    {
        _handler = handler;
    }

    protected class Handler : Handler<object, TResponse>
    {
    }
}
