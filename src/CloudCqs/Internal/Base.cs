using System.Linq;
using System.Threading.Tasks;
using System;

namespace CloudCqs.Internal
{
    public abstract class Base<TRequest, TResponse> : LogWriter, IRepository<TRequest, TResponse>
        where TRequest : notnull
        where TResponse : notnull
    {
        private BuiltHandler<TResponse>? Handler { get; set; }

        protected Base(CloudCqsOption option) : base(option)
        {
        }

        public async Task<TResponse> Invoke(TRequest request)
        {
            var dataValidation = new Function(nameof(DataAnnotationValidator),
                p =>
                {
                    var error = DataAnnotationValidator.Validate(p);
                    if (error != null) throw new ValidationException(error);
                    return Task.FromResult(p);
                });
            var functions = Handler?.Functions ?? Array.Empty<Function>();

            var response = await functions
                .Prepend(dataValidation)
                .Aggregate(
                    Task.FromResult(request as object),
                    async (acc, cur) => await Trace(
                        cur.Description,
                        await acc,
                        req => cur.Func(req)));

            if (response is TResponse res) return res;
            throw new TypeGuardException(typeof(TResponse), response);
        }

        protected void SetHandler(BuiltHandler<TResponse> handler)
        {
            Handler = handler;
        }
    }
}
