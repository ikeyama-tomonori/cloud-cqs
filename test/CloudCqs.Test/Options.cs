using System;

namespace CloudCqs.Test;

public static class Options
{
    public static CloudCqsOptions Instance => new()
    {
        RepositoryExecuted = p => Console.WriteLine(
            $"Executed: {p.repositoryType.FullName} request={p.request}, response={p.response} in {p.timeSpan.TotalMilliseconds}ms"),
        RepositoryTerminated = p => Console.WriteLine(
            $"Terminated: {p.repositoryType.FullName} request={p.request}, exception={p.exception} in {p.timeSpan.TotalMilliseconds}ms"),
        FunctionExecuted = p => Console.WriteLine(
            $"Executed: {p.repositoryType.FullName}[{p.description}] request={p.param}, response={p.result} in {p.timeSpan.TotalMilliseconds}ms"),
        FunctionTerminated = p => Console.WriteLine(
            $"Terminated: {p.repositoryType.FullName}[{p.description}] request={p.param}, exception={p.exception} in {p.timeSpan.TotalMilliseconds}ms"),
    };
}
