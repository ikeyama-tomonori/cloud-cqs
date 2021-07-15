using System;
using System.Linq;
using System.Threading.Tasks;

namespace CloudCqs.CommandFacade
{
    public static class HandlerExtensions
    {
        public static BuiltHandler Build<TProps>
            (this Handler<TProps, object> handler)
            where TProps : notnull
            => Command.HandlerExtensions.Build(handler);
    }
}
