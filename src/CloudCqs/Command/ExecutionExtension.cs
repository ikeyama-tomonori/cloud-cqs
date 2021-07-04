using System.Linq;
using System.Threading.Tasks;
using System;

namespace CloudCqs.Command
{
    using Internal;

    public static class ExecutionExtension
    {
        public static ExecutionCompleted<object> Build<TProps>(this Execution<TProps, object> execution)
            where TProps : notnull
            => new(execution.Functions.Append(new(nameof(Build), async _ => await Task.Run(() => new object()))).ToArray());
    }
}
