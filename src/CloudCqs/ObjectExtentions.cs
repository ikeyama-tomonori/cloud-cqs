using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CloudCqs
{
    public static class ObjectExtentions
    {
        public static void Validate(this object target)
        {
            var context = new ValidationContext(target, null, null);
            var results = new List<ValidationResult>();

            if (Validator.TryValidateObject(target, context, results, true))
            {
                return;
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
        }
    }
}
