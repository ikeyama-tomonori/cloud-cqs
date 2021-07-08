using System;
using System.Runtime.Serialization;

namespace CloudCqs
{
    [Serializable]
    public class ValidationException : Exception
    {
        public ValidationError Error { get; }

        public ValidationException(ValidationError error)
        {
            Error = error;
        }

        protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Error = null!;
        }
    }
}
