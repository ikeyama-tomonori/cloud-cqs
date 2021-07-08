using System.Linq;
using System.Threading.Tasks;

namespace CloudCqs.Command
{
    using Internal;

    public static class ExecutionExtension
    {
        public static BuiltHandler<object> Build<TProps>(this Handler<TProps, object> execution)
            where TProps : notnull
            => new(execution.Functions.Append(new(nameof(Build), _ => Task.FromResult(new object()))).ToArray());
    }
}
