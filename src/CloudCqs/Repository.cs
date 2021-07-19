using System;
using System.Linq;
using System.Threading.Tasks;

namespace CloudCqs
{
    public abstract class Repository<TRequest, TResponse> : LogWriter, IRepository<TRequest, TResponse>
        where TRequest : notnull
        where TResponse : notnull
    {
        private BuiltHandler? Handler { get; set; }

        protected Repository(CloudCqsOptions option) : base(option)
        {
        }

        public async Task<TResponse> Invoke(TRequest request)
        {
            var dataValidation = new Function("Validate request data by annotations",
                props =>
                {
                    props.Validate();
                    return Task.FromResult(props);
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

        protected void SetHandler(BuiltHandler handler)
        {
            Handler = handler;
        }
    }
}
