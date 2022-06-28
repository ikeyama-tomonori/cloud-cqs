namespace CloudCqs;

using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Runtime.Serialization;

[Serializable]
public class StatusCodeException : Exception
{
    public StatusCodeException(HttpStatusCode httpStatusCode, ValidationResult validationResult)
    {
        this.HttpStatusCode = httpStatusCode;
        this.ValidationResult = validationResult;
    }

    protected StatusCodeException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }

    public HttpStatusCode HttpStatusCode { get; } = default;
    public ValidationResult ValidationResult { get; } = new(null);
    public override string Message => this.ValidationResult.ErrorMessage ?? base.Message;
}
