namespace gpconnect_appointment_checker.Helpers.Constants
{
    public class ValidationConstants
    {
        public const string AlphaNumericCharactersOnly = @"[^a-zA-Z0-9, ]";

        public const string AlphaNumericCharactersWithLeadingTrailingSpacesAndCommaSpaceOnly =
            @"^\s*[a-zA-Z0-9, ]*\s*$";

        public const string NhsNetEmailAddress = @"^[a-zA-Z0-9_.+-]+@(?:(?:[a-zA-Z0-9-]+\.)?[a-zA-Z]+\.)?nhs.net$";
    }
}
