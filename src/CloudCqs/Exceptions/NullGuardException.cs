namespace CloudCqs;

using System.Runtime.Serialization;

[Serializable]
public class NullGuardException : Exception
{
    public NullGuardException(string name) : base($"Value can not be null: {name}") { }

    protected NullGuardException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
