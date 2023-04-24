namespace CloudCqs.NewId;

public static class HandlerExtensions
{
    public static Handler<IEnumerable<TResult>, TResponse> ThenIterate<TResult, TResponse, TParam>(
        this Handler<IEnumerable<TParam>, TResponse> handler,
        string description,
        Func<TParam, ValueTask<TResult>> func
    )
        where TParam : notnull
        where TResult : notnull
        where TResponse : notnull =>
        Query.HandlerExtensions.ThenIterate(handler, description, func);

    public static Handler<object, TResponse> ThenIterate<TResponse, TParam>(
        this Handler<IEnumerable<TParam>, TResponse> handler,
        string description,
        Func<TParam, ValueTask> func
    )
        where TParam : notnull
        where TResponse : notnull =>
        Query.HandlerExtensions.ThenIterate(handler, description, func);

    public static Handler<IEnumerable<TResult>, TResponse> ThenIterate<TResult, TResponse, TParam>(
        this Handler<IEnumerable<TParam>, TResponse> handler,
        string description,
        Func<TParam, TResult> func
    )
        where TParam : notnull
        where TResult : notnull
        where TResponse : notnull =>
        Query.HandlerExtensions.ThenIterate(handler, description, func);

    public static Handler<object, TResponse> ThenIterate<TResponse, TParam>(
        this Handler<IEnumerable<TParam>, TResponse> handler,
        string description,
        Action<TParam> func
    )
        where TParam : notnull
        where TResponse : notnull =>
        Query.HandlerExtensions.ThenIterate(handler, description, func);
}
