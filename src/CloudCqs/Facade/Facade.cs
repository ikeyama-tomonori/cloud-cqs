namespace CloudCqs.Facade
{
    public abstract class Facade<TRequest, TResponse> : Repository<TRequest, TResponse>, IFacade<TRequest, TResponse>
        where TRequest : notnull
        where TResponse : notnull
    {
        protected class Handler : Handler<TRequest, TResponse>
        {
        }

        protected Facade(CloudCqsOptions option) : base(option)
        {
        }
    }
}
