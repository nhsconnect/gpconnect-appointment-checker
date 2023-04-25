using GpConnect.AppointmentChecker.Api.Helpers;
using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Organisation;

public class OrganisationAddress
{
    private string[] _AddressLines;

    [JsonProperty("line")]
    public string[] AddressLines
    {
        internal get { return _AddressLines?.Select(x => x.ToTitleCase()).ToArray(); }
        set { _AddressLines = value; }
    }

    public string AddressLine1
    {
        get { return AddressLines != null && AddressLines.Length >= 0 ? AddressLines[0] : null; }
    }

    public string AddressLine2
    {
        get { return AddressLines != null && AddressLines.Length > 1 ? AddressLines[1] : null; }
    }

    private string _Town;

    [JsonProperty("city")]
    public string Town
    {
        get { return _Town.ToTitleCase(); }
        set { _Town = value; }
    }

    private string _County;

    [JsonProperty("district")]
    public string County
    {
        get { return _County.ToTitleCase(); }
        set { _County = value; }
    }

    private string _Country;

    [JsonProperty("country")]
    public string Country
    {
        get { return _Country.ToTitleCase(); }
        set { _Country = value; }
    }

    [JsonProperty("postalCode")]
    public string PostCode { get; set; } = "";

    public string FullAddress => GetFullAddressAsString();

    private string GetFullAddressAsString()
    {
        var addressLines = new List<string> {
            AddressLine1,
            AddressLine2,
            Town,
            County,
            PostCode,
            Country
        };

        return string.Join(", ", addressLines.Where(s => !string.IsNullOrWhiteSpace(s)));
    }
}
