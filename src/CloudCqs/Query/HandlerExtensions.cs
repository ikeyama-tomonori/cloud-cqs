namespace CloudCqs.Query;

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
        handler.Then(
            description,
            async p =>
            {
                var result = new List<TResult>();
                foreach (var item in p)
                {
                    var ret = await func(item);
                    result.Add(ret);
                }
                return result.AsEnumerable();
            }
        );

    public static Handler<object, TResponse> ThenIterate<TResponse, TParam>(
        this Handler<IEnumerable<TParam>, TResponse> handler,
        string description,
        Func<TParam, ValueTask> func
    )
        where TParam : notnull
        where TResponse : notnull =>
        handler.Then(
            description,
            async p =>
            {
                foreach (var item in p)
                {
                    await func(item);
                }
            }
        );

    public static Handler<IEnumerable<TResult>, TResponse> ThenIterate<TResult, TResponse, TParam>(
        this Handler<IEnumerable<TParam>, TResponse> handler,
        string description,
        Func<TParam, TResult> func
    )
        where TParam : notnull
        where TResult : notnull
        where TResponse : notnull =>
        handler.ThenIterate(description, param => ValueTask.FromResult(func(param)));

    public static Handler<object, TResponse> ThenIterate<TResponse, TParam>(
        this Handler<IEnumerable<TParam>, TResponse> handler,
        string description,
        Action<TParam> func
    )
        where TParam : notnull
        where TResponse : notnull =>
        handler.Then(
            description,
            p =>
            {
                foreach (var item in p)
                {
                    func(item);
                }
            }
        );
}
