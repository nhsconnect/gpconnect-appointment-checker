using Newtonsoft.Json;
using GpConnect.AppointmentChecker.Api.Helpers;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Spine;

public class Organisation
{
    public int OrganisationId { get; set; }

    [JsonProperty("nhsIDCode")]
    public string OdsCode { get; set; }

    [JsonProperty("o")]
    public string OrganisationName { get; set; }

    [JsonProperty("postalAddress")]
    public string PostalAddress { get; set; }

    public string[] PostalAddressFields => PostalAddress.Split(new char[] { ',', '$' });

    [JsonProperty("postalCode")]
    public string PostalCode { get; set; }

    [JsonProperty("nhsOrgTypeCode")]
    public string OrganisationTypeCode { get; set; }

    public string FormattedOrganisationDetails => $"{OrganisationName} ({OdsCode}) - {AddressBuilder.GetAddress(PostalAddressFields.ToList(), PostalCode)}";

    public string OrganisationLocation => $"{OrganisationName}, {AddressBuilder.GetAddress(PostalAddressFields.ToList())}";

    public string OrganisationLocationWithOdsCode => $"{OrganisationName}, {AddressBuilder.GetAddress(PostalAddressFields.ToList())} ({OdsCode})";
}
