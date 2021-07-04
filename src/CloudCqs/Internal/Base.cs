using System;
using System.Threading.Tasks;
using System.Linq;

namespace CloudCqs.Internal
{
    public abstract class Base<TRequest, TResponse> : LogWriter, IRepository<TRequest, TResponse>
        where TRequest : notnull
        where TResponse : notnull
    {
        private ExecutionCompleted<TResponse>? execution;

        protected Base(LogContext logContext) : base(logContext)
        {
        }

        public async Task<TResponse> Invoke(TRequest request)
        {
            if (execution == null) throw new NullGuardException(nameof(execution));

            var response = default(TResponse);
            await Trace(
                $"{GetType().FullName}",
                async () =>
                {
                    var dataValidation = new FunctionBlock(nameof(DataAnnotationValidator),
                        async p => await Task.Run(() =>
                        {
                            var error = DataAnnotationValidator.Validate(p);
                            if (error != null) throw new ValidationException(error);
                            return p;
                        }));

                    object? last = request;

                    foreach (var func in execution.Functions.Prepend(dataValidation))
                    {
                        await Trace(
                            func.Description, async () =>
                            {
                                last = await func.Func(last);
                            },
                            () => last,
                            () => last);
                    }

                    if (last is TResponse res)
                    {
                        response = res;
                    }
                    else
                    {
                        throw new TypeGuardException(typeof(TResponse), last);
                    }
                },
                () => request,
                () => response);

            if (response is TResponse res) return res;
            throw new TypeGuardException(typeof(TResponse), response);
        }

        protected void SetExecution(ExecutionCompleted<TResponse> execution)
        {
            this.execution = execution;
        }
    }
}
