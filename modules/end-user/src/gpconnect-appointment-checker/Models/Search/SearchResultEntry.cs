using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GpConnect.AppointmentChecker.Models.Search;

public class SearchResultEntry
{
    [JsonProperty("appointmentDate")]
    public DateTimeOffset? AppointmentDate { get; set; }
    [JsonProperty("sessionName")] 
    public string SessionName { get; set; }
    [JsonProperty("startTime")] 
    public DateTimeOffset? StartTime { get; set; }
    [JsonProperty("duration")] 
    public double Duration { get; set; }
    [JsonProperty("slotType")]
    public string SlotType { get; set; }
    [JsonProperty("deliveryChannel")]
    public string DeliveryChannel { get; set; }
    [JsonProperty("practitionerPrefix")]
    public string PractitionerPrefix { get; set; }
    [JsonProperty("practitionerGivenName")]
    public string PractitionerGivenName { get; set; }
    [JsonProperty("practitionerFamilyName")]
    public string PractitionerFamilyName { get; set; }
    [JsonProperty("practitionerRole")]
    public string PractitionerRole { get; set; }
    [JsonProperty("practitionerGender")]
    public string PractitionerGender { get; set; }
    [JsonProperty("practitionerName")]
    public string PractitionerName { get; set; }
    [JsonProperty("locationName")]
    public string LocationName { get; set; }
    [JsonProperty("locationAddressLines")]    
    public List<string> LocationAddressLines { get; set; }
    [JsonProperty("locationAddressLinesAsString")]
    public string LocationAddressLinesAsString { get; set; }
    [JsonProperty("locationCity")]
    public string LocationCity { get; set; }
    [JsonProperty("locationDistrict")]
    public string LocationDistrict { get; set; }
    [JsonProperty("locationPostalCode")]
    public string LocationPostalCode { get; set; }
    [JsonProperty("locationCountry")]
    public string LocationCountry { get; set; }
    [JsonProperty("slotInPast")]
    public bool SlotInPast { get; set; }
    [JsonProperty("noAddressProvided")]
    public bool NoAddressProvided { get; set; }
}
