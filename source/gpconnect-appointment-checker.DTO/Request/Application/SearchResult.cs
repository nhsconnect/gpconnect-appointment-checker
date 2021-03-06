﻿namespace gpconnect_appointment_checker.DTO.Request.Application
{
    public class SearchResult
    {
        public int SearchGroupId { get; set; }
        public string ProviderCode { get; set; }
        public string ConsumerCode { get; set; }
        public int ErrorCode { get; set; }
        public string Details { get; set; }
        public string ProviderPublisher { get; set; }
        public int? SpineMessageId { get; set; }
        public double SearchDurationSeconds { get; set; }
    }
}
