using System;
using System.Linq;
using System.Threading.Tasks;

namespace CloudCqs;

public class Handler<TParam, TResponse>
    where TParam : notnull
{
    internal Function[] Functions { get; }

    internal Handler(Function[] functions)
    {
        Functions = functions;
    }

    internal Handler() : this(Array.Empty<Function>())
    {
    }

    public Handler<TResult, TResponse> Then<TResult>(string description, Func<TParam, Task<TResult>> func)
        where TResult : notnull
    {
        var thisFunction = new Function(
            Description: description,
            Func: async param =>
            {
                if (param is TParam p)
                {
                    return await func(p);
                }
                throw new TypeGuardException(typeof(TParam), param);
            }
        );
        return new(Functions.Append(thisFunction).ToArray());
    }

    public Handler<TResult, TResponse> Then<TResult>(string description, Func<Task<TResult>> func)
        where TResult : notnull
    => Then(description, _ => func());

    public Handler<Void, TResponse> Then(string description, Func<TParam, Task> func)
    => Then(description, async p =>
    {
        await func(p);
        return Void.Value;
    });

    public Handler<Void, TResponse> Then(string description, Func<Task> func)
    => Then(description, _ => func());

    public Handler<TResult, TResponse> Then<TResult>(string description, Func<TParam, TResult> func)
        where TResult : notnull
    => Then(description, param => Task.FromResult(func(param)));

    public Handler<TResult, TResponse> Then<TResult>(string description, Func<TResult> func)
        where TResult : notnull
    => Then(description, _ => func());

    public Handler<Void, TResponse> Then(string description, Action<TParam> func)
        => Then(description, p =>
        {
            func(p);
            return Void.Value;
        });

    public Handler<Void, TResponse> Then(string description, Action func)
    => Then(description, _ => func());
}
