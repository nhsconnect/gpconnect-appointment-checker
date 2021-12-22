namespace gpconnect_appointment_checker.DTO.Response.Configuration
{
    public class General
    {
        public string ProductName { get; set; }
        public string ProductVersion { get; set; }
        public int MaxNumWeeksSearch { get; set; }
        public int MaxNumberProviderCodesSearch { get; set; }
        public int MaxNumberConsumerCodesSearch { get; set; }
        public int LogRetentionDays { get; set; }
        public string GetAccessEmailAddress { get; set; }
    }
}
