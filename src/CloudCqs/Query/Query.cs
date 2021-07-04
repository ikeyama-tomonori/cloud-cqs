using System;

namespace CloudCqs.Query
{
    using Internal;

    public abstract class Query<TRequest, TResponse> : Base<TRequest, TResponse>, IQuery<TRequest, TResponse>
        where TRequest : notnull
        where TResponse : notnull
    {
        protected class Execution : Execution<TRequest, TResponse>
        {
        }

        protected Query(LogContext logContext) : base(logContext)

        {
        }
    }
}
