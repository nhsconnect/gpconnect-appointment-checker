﻿namespace gpconnect_appointment_checker.Helpers.Constants
{
    public class ValidationConstants
    {
        public const string ALPHANUMERICCHARACTERSONLY = @"[^a-zA-Z0-9, ]";
        public const string ALPHANUMERICCHARACTERSWITHLEADINGTRAILINGSPACESANDCOMMASPACEONLY = @"^\s*[a-zA-Z0-9, ]*\s*$";
    }
}
