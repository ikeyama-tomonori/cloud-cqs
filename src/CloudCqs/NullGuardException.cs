using System;
using System.Runtime.Serialization;

namespace CloudCqs
{
    [Serializable]
    public class NullGuardException : Exception
    {
        public NullGuardException(string name) : base($"Value can not be null: {name}")
        {
        }

        protected NullGuardException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
