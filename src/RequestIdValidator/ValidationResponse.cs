using System.Net;

namespace RequestIdValidator
{
    public class ValidationResponse
    {
        public ValidationResponse(ValidationResult status)
        {
            Status = status;
        }

        public ValidationResult Status { get; set; }

        public void ThrowExceptionOnError()
        {
            if (Status == ValidationResult.ErrorMissingBody)
            {
                throw new IdentityException(HttpStatusCode.BadRequest, Status, "Missing request body");
            }

            if (Status == ValidationResult.ErrorMissingOrInvalidLamda)
            {
                throw new IdentityException(HttpStatusCode.InternalServerError, Status, "Missing or invalid lamda property member expression");
            }

            if (Status == ValidationResult.ErrorUnequalIds)
            {
                throw new IdentityException(HttpStatusCode.BadRequest, Status, "Invalid request");
            }

            if (Status != ValidationResult.Valid)
            {
                throw new IdentityException(HttpStatusCode.BadRequest, Status, "Invalid request");
            }
        }
    }
}