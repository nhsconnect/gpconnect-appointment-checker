namespace GpConnect.AppointmentChecker.Api.DTO.Request;

public class SearchRequest
{
    public string ProviderOdsCode { get; set; }
    public string ConsumerOdsCode { get; set; }
    public string? ConsumerOrganisationType { get; set; }
    public string DateRange { get; set; }    

    public DateTime StartDate => Convert.ToDateTime(DateRange.Split(":")[0]);
    public DateTime EndDate => Convert.ToDateTime(DateRange.Split(":")[1]);

    public Uri RequestUri { get; set; }

    public int UserId { get; set; }
    public int UserSessionId { get; set; }
    public string Sid { get; set; }

    public List<string> ProviderOdsCodeAsList => ProviderOdsCode?.Split(',', ' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
    public List<string> ConsumerOdsCodeAsList => ConsumerOdsCode?.Split(',', ' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

    public bool HasMultipleProviderOdsCodes => ProviderOdsCodeAsList?.Count > 1;
    public bool HasMultipleConsumerOdsCodes => ConsumerOdsCodeAsList?.Count > 1;

    public bool IsMultiSearch => HasMultipleProviderOdsCodes || HasMultipleConsumerOdsCodes;

    public bool ValidSearchCombination => ((!HasMultipleProviderOdsCodes && !HasMultipleConsumerOdsCodes)
                                               || (HasMultipleConsumerOdsCodes && !HasMultipleProviderOdsCodes)
                                               || (HasMultipleProviderOdsCodes && !HasMultipleConsumerOdsCodes));
}
