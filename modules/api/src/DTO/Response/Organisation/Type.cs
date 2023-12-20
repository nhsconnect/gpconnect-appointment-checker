using Newtonsoft.Json;
using System.Collections.Generic;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Organisation;

public class Type
{
    [JsonProperty("coding")]
    public OrganisationCoding Coding { get; set; }
}