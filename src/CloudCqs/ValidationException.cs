using System;
using System.Collections.Generic;

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
    }
}
