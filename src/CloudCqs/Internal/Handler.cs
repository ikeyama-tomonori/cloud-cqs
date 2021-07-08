using System;
using System.Linq;
using System.Threading.Tasks;

namespace CloudCqs.Internal
{
    public class Handler<TProps, TResponse>
        where TProps : notnull
    {
        internal Function[] Functions { get; }

        internal Handler(Function[] functions)
        {
            Functions = functions;
        }

        internal Handler() : this(Array.Empty<Function>())
        {
        }

        public Handler<TResult, TResponse> Then<TResult>(string description, Func<TProps, Task<TResult>> func)
            where TResult : notnull
        {
            var thisFunction = new Function(
                Description: description,
                Func: async props =>
                {
                    if (props is TProps p)
                    {
                        return await func(p);
                    }
                    throw new TypeGuardException(typeof(TProps), props);
                }
            );
            return new(Functions.Append(thisFunction).ToArray());
        }

        public Handler<TResult, TResponse> Then<TResult>(string description, Func<Task<TResult>> func)
            where TResult : notnull
        => Then(description, _ => func());

        public Handler<object, TResponse> Then(string description, Func<TProps, Task> func)
        => Then(description, async p =>
        {
            await func(p);
            return new object();
        });

        public Handler<object, TResponse> Then(string description, Func<Task> func)
        => Then(description, _ => func());

        public Handler<TResult, TResponse> Then<TResult>(string description, Func<TProps, TResult> func)
            where TResult : notnull
        => Then(description, props => Task.FromResult(func(props)));

        public Handler<TResult, TResponse> Then<TResult>(string description, Func<TResult> func)
            where TResult : notnull
        => Then(description, _ => func());

        public Handler<object, TResponse> Then(string description, Action<TProps> func)
            => Then(description, p =>
            {
                func(p);

                return new object();
            });

        public Handler<object, TResponse> Then(string description, Action func)
        => Then(description, _ => func());

        public Handler<TProps, TResponse> Validate(string description, Func<TProps, ValidationError?> func)
        {
            var thisFunction = new Function
            (
                Description: description,
                Func: props =>
               {
                   if (props is TProps p)
                   {
                       var error = func(p);
                       if (error != null) throw new ValidationException(error);
                       return Task.FromResult(props);
                   }
                   throw new TypeGuardException(typeof(TProps), props);
               }
            );
            return new(Functions.Append(thisFunction).ToArray());
        }
    }
}
