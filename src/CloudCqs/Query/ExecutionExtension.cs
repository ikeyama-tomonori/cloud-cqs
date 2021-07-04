namespace CloudCqs.Query
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
