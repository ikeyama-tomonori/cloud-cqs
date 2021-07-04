using System;

namespace CloudCqs.NewId
{
    using Internal;

    public abstract class NewId<TRequest, TKey> : Base<TRequest, TKey>, INewId<TRequest, TKey>
        where TRequest : notnull
        where TKey : notnull
    {
        protected class Execution : Execution<TRequest, TKey>
        {
        }

        protected NewId(LogContext logContext) : base(logContext)
        {
        }
    }
}
