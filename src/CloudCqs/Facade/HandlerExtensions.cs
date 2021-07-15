using System;
using System.Linq;
using System.Threading.Tasks;

namespace CloudCqs.Facade
{
    public static class HandlerExtensions
    {
        public static BuiltHandler Build<TResponse>
            (this Handler<TResponse, TResponse> handler)
            where TResponse : notnull
        => Query.HandlerExtensions.Build(handler);
    }
}
