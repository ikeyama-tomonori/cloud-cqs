namespace CloudCqs.Query
{
    public static class HandlerExtensions
    {
        public static BuiltHandler Build<TResponse>
            (this Handler<TResponse, TResponse> handler)
            where TResponse : notnull
        => new(handler.Functions);
    }
}
