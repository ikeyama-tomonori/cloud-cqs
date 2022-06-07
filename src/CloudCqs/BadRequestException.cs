namespace CloudCqs;

[Serializable]
public class BadRequestException : StatusCodeException
{
    public BadRequestException(Dictionary<string, string[]>? errors = null)
        : base(HttpStatusCode.BadRequest, errors)
    {
    }

    protected BadRequestException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
