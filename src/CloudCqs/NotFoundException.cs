namespace CloudCqs;

[Serializable]
public class NotFoundException : StatusCodeException
{
    public NotFoundException(Dictionary<string, string[]>? errors = null)
        : base(HttpStatusCode.NotFound, errors)
    {
    }

    protected NotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
