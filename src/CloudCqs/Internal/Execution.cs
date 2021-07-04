using System;
using System.Linq;
using System.Threading.Tasks;

namespace CloudCqs.Internal
{
    public class Execution<TProps, TResponse>
        where TProps : notnull
    {
        internal Execution(FunctionBlock[] functions)
        {
            Functions = functions;
        }

        internal Execution()
        {
            Functions = Array.Empty<FunctionBlock>();
        }

        internal FunctionBlock[] Functions { get; }

        public Execution<TResult, TResponse> Then<TResult>(string description, Func<TProps, Task<TResult>> func)
            where TResult : notnull
        {
            var thisFunction = new FunctionBlock(
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

        public Execution<TResult, TResponse> Then<TResult>(string description, Func<Task<TResult>> func)
            where TResult : notnull
        => Then(description, _ => func());

        public Execution<object, TResponse> Then(string description, Func<TProps, Task> func)
        => Then(description, async p =>
        {
            await func(p);
            return new object();
        });

        public Execution<object, TResponse> Then(string description, Func<Task> func)
        => Then(description, _ => func());

        public Execution<TResult, TResponse> Then<TResult>(string description, Func<TProps, TResult> func)
            where TResult : notnull
        => Then(description, props => Task.Run(() => func(props)));

        public Execution<TResult, TResponse> Then<TResult>(string description, Func<TResult> func)
            where TResult : notnull
        => Then(description, _ => func());

        public Execution<object, TResponse> Then(string description, Action<TProps> func)
            => Then(description, p =>
            {
                func(p);

                return new object();
            });

        public Execution<object, TResponse> Then(string description, Action func)
        => Then(description, _ => func());

        public Execution<TProps, TResponse> Validate(string description, Func<TProps, ValidationError?> func)
        {
            var thisFunction = new FunctionBlock
            (
                Description: description,
                Func: async props =>
               {
                   if (props is TProps p)
                   {
                       var error = await Task.Run(() => func(p));
                       if (error != null) throw new ValidationException(error);
                       return props;
                   }
                   throw new TypeGuardException(typeof(TProps), props);
               }
            );
            return new(Functions.Append(thisFunction).ToArray());
        }
    }
}
