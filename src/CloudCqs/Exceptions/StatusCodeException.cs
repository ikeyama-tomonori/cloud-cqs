using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Runtime.Serialization;

namespace CloudCqs;

[Serializable]
public class StatusCodeException : Exception
{
    public HttpStatusCode HttpStatusCode { get; } = default;
    public ValidationResult ValidationResult { get; } = new(null);

    public override string Message => ValidationResult.ErrorMessage ?? base.Message;

    public StatusCodeException(
        HttpStatusCode httpStatusCode,
         ValidationResult validationResult)
    {
        HttpStatusCode = httpStatusCode;
        ValidationResult = validationResult;
    }

    protected StatusCodeException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {

    }
}
