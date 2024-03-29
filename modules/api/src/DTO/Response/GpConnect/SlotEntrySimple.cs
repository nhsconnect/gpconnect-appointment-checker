﻿using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class SlotEntrySimple
{
    [JsonProperty("Appointment Date")]
    public DateTimeOffset? AppointmentDate { get; set; }
    [JsonProperty("Session Name")] 
    public string SessionName { get; set; }
    [JsonProperty("Start Time")] 
    public DateTimeOffset? StartTime { get; set; }
    [JsonProperty("Duration")] 
    public double Duration { get; set; }
    [JsonProperty("Slot Type")]
    public string SlotType { get; set; }
    [JsonProperty("Delivery Channel")]
    public string DeliveryChannel { get; set; }
    [JsonProperty("Practitioner Prefix")]
    public string PractitionerPrefix { get; set; }
    [JsonProperty("Practitioner Given Name")]
    public string PractitionerGivenName { get; set; }
    [JsonProperty("Practitioner Family Name")]
    public string PractitionerFamilyName { get; set; }
    [JsonProperty("Practitioner Role")]
    public string PractitionerRole { get; set; }
    [JsonProperty("Practitioner Gender")]
    public string PractitionerGender { get; set; }
    [JsonProperty("Location Name")]
    public string LocationName { get; set; }
    [IgnoreDataMember]
    public List<string> LocationAddressLines { get; set; }
    [JsonProperty("Location Address")]
    public string LocationAddressLinesAsString { get; set; }
    [JsonProperty("Location City")]
    public string LocationCity { get; set; }
    [JsonProperty("Location District")]
    public string LocationDistrict { get; set; }
    [JsonProperty("Location Postal Code")]
    public string LocationPostalCode { get; set; }
    [JsonProperty("Location Country")]
    public string LocationCountry { get; set; }

    [IgnoreDataMember]
    public List<string> PractitionerDetails { get; set; }
    [JsonProperty("Practitioner Name")]
    public string PractitionerName => PractitionerBuilder(PractitionerDetails, PractitionerFamilyName, PractitionerGivenName, PractitionerPrefix);

    [IgnoreDataMember]
    public bool SlotInPast { get; set; }

    [IgnoreDataMember]
    public bool NoAddressProvided => (LocationAddressLines == null || LocationAddressLines?.Count == 0)
                                     && string.IsNullOrEmpty(LocationCity) &&
                                     string.IsNullOrEmpty(LocationDistrict) &&
                                     string.IsNullOrEmpty(LocationPostalCode) &&
                                     string.IsNullOrEmpty(LocationCountry);

    private static string PractitionerBuilder(List<string> practitionerDetails, string familyName, string givenName, string prefix)
    {
        practitionerDetails ??= new List<string>();
        if (!string.IsNullOrEmpty(familyName)) { practitionerDetails.Add($"{familyName.ToUpper()},"); }
        if (!string.IsNullOrEmpty(givenName)) { practitionerDetails.Add($"{givenName}"); }
        if (!string.IsNullOrEmpty(prefix)) { practitionerDetails.Add($"({prefix})"); }
        return string.Join(" ", practitionerDetails);
    }
}
