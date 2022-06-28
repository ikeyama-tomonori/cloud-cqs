namespace CloudCqs.Test;

public static class Options
{
    public static CloudCqsOptions Instance =>
        new()
        {
            RepositoryExecuted = p =>
                Console.WriteLine(
                    $"Executed: {p.RepositoryType.FullName} request={p.Request}, response={p.Response} in {p.TimeSpan.TotalMilliseconds}ms"
                ),
            RepositoryTerminated = p =>
                Console.WriteLine(
                    $"Terminated: {p.RepositoryType.FullName} request={p.Request}, exception={p.Exception} in {p.TimeSpan.TotalMilliseconds}ms"
                ),
            FunctionExecuted = p =>
                Console.WriteLine(
                    $"Executed: {p.RepositoryType.FullName}[{p.Description}] request={p.Param}, response={p.Result} in {p.TimeSpan.TotalMilliseconds}ms"
                ),
            FunctionTerminated = p =>
                Console.WriteLine(
                    $"Terminated: {p.RepositoryType.FullName}[{p.Description}] request={p.Param}, exception={p.Exception} in {p.TimeSpan.TotalMilliseconds}ms"
                ),
        };
}
