namespace CloudCqs.Query
{
    using Internal;

    public abstract class Query<TRequest, TResponse> : Base<TRequest, TResponse>, IQuery<TRequest, TResponse>
        where TRequest : notnull
        where TResponse : notnull
    {
        protected class Handler : Handler<TRequest, TResponse>
        {
        }

        protected Query(CloudCqsOption option) : base(option)

        {
        }
    }
}
