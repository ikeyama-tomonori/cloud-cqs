using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
                    var context = new ValidationContext(props, null, null);
                    var results = new List<ValidationResult>();

                    if (Validator.TryValidateObject(props, context, results, true))
                    {
                        return Task.FromResult(props);
                    }

                    var errors = results
                        .SelectMany(
                            result => result
                                .MemberNames
                                .Select(name => (name, message: result.ErrorMessage ?? "")))
                        .GroupBy(member => member.name)
                        .ToDictionary(
                            grouping => grouping.Key,
                            grouping => grouping
                                .Select(member => member.message)
                                .ToArray());

                    throw new BadRequestException(errors);
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
