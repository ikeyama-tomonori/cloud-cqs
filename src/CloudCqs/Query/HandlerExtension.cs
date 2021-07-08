namespace CloudCqs.Query
{
    using Internal;

    public static class HandlerExtension
    {
        public static BuiltHandler<TResponse> Build<TResponse>
            (this Handler<TResponse, TResponse> execution)
            where TResponse : notnull
        => new(execution.Functions);
    }
}
