namespace CloudCqs;

using System.Runtime.Serialization;

[Serializable]
public class TypeGuardException : Exception
{
    public TypeGuardException(Type expected, object? actual)
        : base($"{expected.Name} is expected but {actual}") { }

    protected TypeGuardException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
