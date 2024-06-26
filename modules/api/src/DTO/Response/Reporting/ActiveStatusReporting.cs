﻿using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;
using GpConnect.AppointmentChecker.Api.DTO.Response.Organisation.Hierarchy;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using Newtonsoft.Json;

namespace gpconnect_appointment_checker.api.DTO.Response.Reporting;

public abstract class ActiveStatusReporting
{
    [JsonIgnore]
    public Hierarchy Hierarchy { get; set; }

    [JsonProperty("ODS_Code")]
    public string OdsCode { get; set; }

    [JsonProperty("Supplier_Name")]
    public string SupplierName { get; set; }

    [JsonProperty("Site_Name")]
    public string SiteName => Hierarchy != null ? Hierarchy.SiteName : string.Empty;

    [JsonProperty("Postcode")]
    public string Postcode => Hierarchy != null ? Hierarchy.Postcode : string.Empty;

    [JsonProperty("ICB")]
    public string Icb => Hierarchy != null ? string.Format("{0}{1}", Hierarchy.IcbName, Hierarchy.IcbCode != null ? $" ({Hierarchy.IcbCode})" : string.Empty) : string.Empty;

    [JsonProperty("Higher_Health_Authority")]
    public string HigherHealthAuthority => Hierarchy != null ? string.Format("{0}{1}", Hierarchy?.HigherHealthAuthorityName, Hierarchy.HigherHealthAuthorityCode != null ? $" ({Hierarchy.HigherHealthAuthorityCode})" : string.Empty) : string.Empty;

    [JsonProperty("Commissioning_Region")]
    public string NationalGrouping => Hierarchy != null ? string.Format("{0}{1}", Hierarchy.NationalGroupingName, Hierarchy.NationalGroupingCode != null ? $" ({Hierarchy.NationalGroupingCode})" : string.Empty) : string.Empty;

    [JsonProperty("Version")]
    public string ApiVersion { get; set; }

    [JsonIgnore]
    public List<Rest>? Rest { get; set; }

    [JsonIgnore]
    public IEnumerable<string> Operation => Rest?.FirstOrDefault()?.Operation?.Select(x => x.Name);
    
    [JsonProperty("Status")]
    public string Status => Rest != null && Rest.Any(x => x.Operation != null) ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE;
}