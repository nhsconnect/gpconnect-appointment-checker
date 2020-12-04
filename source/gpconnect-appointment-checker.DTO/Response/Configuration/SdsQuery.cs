namespace gpconnect_appointment_checker.DTO.Response.Configuration
{
    public class SdsQuery
    {
        public string QueryName { get; set; }
        public string SearchBase { get; set; }
        public string QueryText { get; set; }
        public string QueryAttributes { get; set; }
        public string[] QueryAttributesAsArray => QueryAttributes?.Replace(" ", "").Split(",");
    }
}
