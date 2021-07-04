using System;
using System.Threading.Tasks;

namespace CloudCqs.Internal
{
    public record FunctionBlock(string Description, Func<object?, Task<object?>> Func);
}
