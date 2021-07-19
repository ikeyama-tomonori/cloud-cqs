using System.Linq;
using System.Threading.Tasks;

namespace CloudCqs.Command
{
    public static class HandlerExtensions
    {
        public static BuiltHandler Build<TProps>
            (this Handler<TProps, Void> handler)
            where TProps : notnull
            => new(handler
                .Functions
                .Append(new(nameof(Build), _ => Task.FromResult(Void.Value as object)))
                .ToArray());
    }
}
