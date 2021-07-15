using System.Linq;
using System.Threading.Tasks;

namespace CloudCqs.Command
{
    public static class HandlerExtensions
    {
        public static BuiltHandler Build<TProps>
            (this Handler<TProps, object> handler)
            where TProps : notnull
            => new(handler
                .Functions
                .Append(new(nameof(Build), _ => Task.FromResult(new object())))
                .ToArray());
    }
}
