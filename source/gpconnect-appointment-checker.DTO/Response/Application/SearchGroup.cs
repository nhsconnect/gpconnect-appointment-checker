using System;

namespace gpconnect_appointment_checker.DTO.Response.Application
{
    public class SearchGroup
    {
        public int SearchGroupId { get; set; }
        public string ProviderOdsTextbox { get; set; }
        public string ConsumerOdsTextbox { get; set; }
        public string SelectedDateRange { get; set; }
        public DateTime SearchStartAt { get; set; }
        public DateTime SearchEndAt { get; set; }
    }
}
