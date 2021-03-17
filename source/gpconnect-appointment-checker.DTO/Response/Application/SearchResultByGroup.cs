namespace gpconnect_appointment_checker.DTO.Response.Application
{
    public class SearchResultByGroup
    {
        public int SearchResultId { get; set; }
        public int SearchGroupId { get; set; }
        public string ProviderOdsCode { get; set; }
        public string ConsumerOdsCode { get; set; }
        public string ProviderOrganisationName { get; set; }
        public string ConsumerOrganisationName { get; set; }
        public int ErrorCode { get; set; }
        public string Details { get; set; }
        public string ProviderPublisher { get; set; }
    }
}
