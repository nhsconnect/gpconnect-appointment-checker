using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using System.Text;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Application;

public class SearchResult
{
    public int SearchResultId { get; set; }
    public int SearchGroupId { get; set; }
    public string ResponsePayload { get; set; }
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
    public string ProviderPublisher { get; set; }
    public double SearchDurationSeconds { get; set; }
    public string ConsumerOrganisationType { get; set; }

    public string SearchAtResults => $"{ProviderOrganisationName} ({ProviderOdsCode}) - {Helpers.AddressBuilder.GetAddress(ProviderAddressFields.ToList(), ProviderPostcode)}";
    public string SearchOnBehalfOfResults => GetSearchOnBehalfOfResults();

    private string GetSearchOnBehalfOfResults()
    {
        var searchOnBehalfOfResultsText = new StringBuilder();

        if (!string.IsNullOrEmpty(ConsumerOrganisationName))
        {
            searchOnBehalfOfResultsText.Append($"{ConsumerOrganisationName} ({ConsumerOdsCode}) - {Helpers.AddressBuilder.GetAddress(ConsumerAddressFields.ToList(), ConsumerPostcode)}");
        }
        if (!string.IsNullOrEmpty(ConsumerOrganisationType))
        {
            searchOnBehalfOfResultsText.Append($"<p>{string.Format(SearchConstants.SEARCHRESULTSSEARCHONBEHALFOFORGTYPETEXT, ConsumerOrganisationType)}</p>");
        }
        return searchOnBehalfOfResultsText.ToString();
    }
}
