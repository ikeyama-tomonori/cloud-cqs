using System;
using System.Collections.Generic;

namespace CloudCqs.Internal
{
    public record ExecutionCompleted<TResponse>(FunctionBlock[] Functions);
}
