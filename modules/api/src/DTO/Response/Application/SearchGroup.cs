namespace GpConnect.AppointmentChecker.Api.DTO.Response.Application;

public class SearchGroup
{
    public int SearchGroupId { get; set; }
    public string ProviderOdsTextbox { get; set; }
    public string ConsumerOdsTextbox { get; set; }
    public string SelectedDateRange { get; set; }
    public string ConsumerOrganisationTypeDropdown { get; set; }
    public DateTime SearchStartAt { get; set; }
    public DateTime? SearchEndAt { get; set; }
}
