using System.Linq;
using System.Threading.Tasks;

namespace CloudCqs.NewId
{
    using Internal;

    public static class ExecutionExtension
    {
        public static ExecutionCompleted<TResponse> Build<TResponse>
            (this Execution<TResponse, TResponse> execution)
            where TResponse : notnull
            => new(execution.Functions);
    }
}
