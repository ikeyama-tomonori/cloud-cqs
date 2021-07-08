namespace CloudCqs.Facade
{
    using Internal;

    public abstract class Facade<TRequest, TResponse> : Base<TRequest, TResponse>, IFacade<TRequest, TResponse>
        where TRequest : notnull
        where TResponse : notnull
    {
        protected class Handler : Handler<TRequest, TResponse>
        {
        }

        protected Facade(CloudCqsOption option) : base(option)
        {
        }
    }
}
