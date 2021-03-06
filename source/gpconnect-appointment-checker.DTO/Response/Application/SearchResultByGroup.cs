﻿namespace gpconnect_appointment_checker.DTO.Response.Application
{
    public class SearchResultByGroup
    {
        public int SearchResultId { get; set; }
        public int SearchGroupId { get; set; }
        public string ProviderOdsCode { get; set; }
        public string ConsumerOdsCode { get; set; }
        public string ProviderOrganisationName { get; set; }
        public string ProviderAddress { get; set; }
        public string[] ProviderAddressFields => ProviderAddress.Split(new char[] { ',' });
        public string ProviderPostcode { get; set; }
        public string ConsumerOrganisationName { get; set; }
        public string ConsumerAddress { get; set; }
        public string[] ConsumerAddressFields => ConsumerAddress.Split(new char[] { ',' });
        public string ConsumerPostcode { get; set; }
        public int ErrorCode { get; set; }
        public string Details { get; set; }
        public string ProviderPublisher { get; set; }
    }
}
