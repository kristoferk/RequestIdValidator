namespace RequestIdValidator
{
    public enum ValidationResult
    {
        Valid = 1,
        ErrorUnequalIds = 2,
        ErrorMissingBody = 3,
        ErrorMissingOrInvalidLamda = 3,
    }
}