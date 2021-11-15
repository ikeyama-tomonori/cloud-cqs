using System;
using System.Linq;
using System.Threading.Tasks;

namespace CloudCqs.CommandFacade
{
    public static class HandlerExtensions
    {
        public static BuiltHandler Build<TParam>
            (this Handler<TParam, Void> handler)
            where TParam : notnull
            => Command.HandlerExtensions.Build(handler);
    }
}
