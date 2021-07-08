using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CloudCqs.Internal
{
    internal static class DataAnnotationValidator
    {
        public static ValidationError? Validate(object? obj)
        {
            if (obj == null) return null;
            var context = new ValidationContext(obj, null, null);
            var results = new List<ValidationResult>();

            if (Validator.TryValidateObject(obj, context, results, true))
            {
                return null;
            }

            var errors = results
                .Where(r => r.ErrorMessage is not null)
                .SelectMany(r => r.MemberNames.Select(n => (n, r.ErrorMessage!)))
                .ToArray();

            return new(errors);
        }
    }
}
