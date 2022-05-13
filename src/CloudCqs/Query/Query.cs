namespace CloudCqs.Query;

public abstract class Query<TRequest, TResponse> : Repository<TRequest, TResponse>, IQuery<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
    protected class Handler : Handler<TRequest, TResponse>
    {
    }

    protected Query(CloudCqsOptions option) : base(option)

    {
    }
}
