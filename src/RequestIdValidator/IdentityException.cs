using System;
using System.Net;

namespace RequestIdValidator
{
    public class IdentityException : Exception
    {
        public IdentityException(HttpStatusCode statusCode, ValidationResult status, string message)
            : base(message)
        {
            StatusCode = statusCode;
            Status = status;
        }

        public ValidationResult Status { get; }

        public HttpStatusCode StatusCode { get; }
    }
}