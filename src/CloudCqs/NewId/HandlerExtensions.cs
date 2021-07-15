namespace CloudCqs.NewId
{
    public static class HandlerExtensions
    {
        public static BuiltHandler Build<TResponse>
            (this Handler<TResponse, TResponse> handler)
            where TResponse : notnull
        => Query.HandlerExtensions.Build(handler);
    }
}
