namespace CloudCqs;

public class Handler<TParam, TResponse>
    where TParam : notnull
{
    internal Function[] Functions { get; }

    internal Handler(Function[] functions)
    {
        Functions = functions;
    }

    // CancellationTokenはFacadeから呼び出す場合に、ライブラリ内からのみ利用するのでアクセスレベルはinternalとする。
    // 利用者はUseCancellationToken()で取得する。
    internal Handler<TResult, TResponse> Then<TResult>(string description, Func<TParam, CancellationToken, Task<TResult>> func)
        where TResult : notnull
    {
        var thisFunction = new Function(
            Description: description,
            Func: async (param, cancellationToken) =>
            {
                if (param is TParam p)
                {
                    return await func(p, cancellationToken);
                }
                throw new TypeGuardException(typeof(TParam), param);
            }
        );
        return new(Functions.Append(thisFunction).ToArray());
    }

    public Handler<TResult, TResponse> Then<TResult>(string description, Func<TParam, Task<TResult>> func)
        where TResult : notnull
    => Then(description, (p, _) => func(p));


    public Handler<object, TResponse> Then(string description, Func<TParam, Task> func)
    => Then(description, async p =>
    {
        await func(p);
        return new object();
    });

    public Handler<TResult, TResponse> Then<TResult>(string description, Func<TParam, TResult> func)
        where TResult : notnull
    => Then(description, param => Task.FromResult(func(param)));

    public Handler<object, TResponse> Then(string description, Action<TParam> func)
    => Then(description, p =>
    {
        func(p);
        return new object();
    });

    public Handler<TParam, TResponse> Validate(string description, Dictionary<string, string[]> errors, Func<TParam, bool> func)
    => Then(description, p =>
    {
        var valid = func(p);
        if (!valid) throw new BadRequestException(errors);
        return p;
    });

    public Handler<TParam, TResponse> Validate(string description, Func<TParam, Dictionary<string, string[]>?> func)
    => Then(description, p =>
    {
        var errors = func(p);
        if (errors != null) throw new BadRequestException(errors);
        return p;
    });

    public Handler<TResult, TResponse> Validate<TResult>(string description, Func<TParam, TResult> func)
        where TResult : notnull
    => Then(description, func);
}
