using System;
using System.Threading.Tasks;

namespace CloudCqs.Internal
{
    public record Function(string Description, Func<object, Task<object>> Func);
}
