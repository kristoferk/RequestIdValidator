namespace RequestIdValidator
{
    public enum ValidationResult
    {
        Valid = 1,
        ErrorNotEqualIds = 2,
        ErrorMissingBody = 3,
        ErrorMissingOrInvalidLamda = 4,
        ErrorMissingIds = 5
    }
}