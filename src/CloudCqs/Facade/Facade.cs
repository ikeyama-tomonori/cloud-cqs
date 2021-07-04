using System;

namespace CloudCqs.Facade
{
    using Internal;

    public abstract class Facade<TRequest, TResponse> : Base<TRequest, TResponse>, IFacade<TRequest, TResponse>
        where TRequest : notnull
        where TResponse : notnull
    {
        protected class Execution : Execution<TRequest, TResponse>
        {
        }

        protected Facade(LogContext logContext) : base(logContext)
        {
        }
    }
}
