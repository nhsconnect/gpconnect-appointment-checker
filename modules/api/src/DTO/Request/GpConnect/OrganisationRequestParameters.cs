namespace GpConnect.AppointmentChecker.Api.DTO.Request.GpConnect;

public class OrganisationRequestParameters
{
    public string OdsCode { get; set; }

    //public int OrganisationId { get; set; }

    //[JsonProperty("nhsIDCode")]

    //[JsonProperty("o")]
    //public string OrganisationName { get; set; }

    //[JsonProperty("postalAddress")]
    //public string PostalAddress { get; set; }

    //public string[] PostalAddressFields => PostalAddress.Split(new char[] { ',', '$' });

    //[JsonProperty("postalCode")]
    //public string PostalCode { get; set; }

    //[JsonProperty("nhsOrgTypeCode")]
    //public string OrganisationTypeCode { get; set; }

    //public string FormattedOrganisationDetails => $"{OrganisationName} ({OdsCode}) - {AddressBuilder.GetAddress(PostalAddressFields.ToList(), PostalCode)}";

    //public string OrganisationLocation => $"{OrganisationName}, {AddressBuilder.GetAddress(PostalAddressFields.ToList(), PostalCode)}";
}
