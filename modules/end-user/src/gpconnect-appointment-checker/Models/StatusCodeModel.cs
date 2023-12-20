namespace gpconnect_appointment_checker.Pages;

public partial class StatusCodeModel : BaseModel
{
  public int? ResponseStatusCode { get; set; } = null;
  public string ResponseStatusCodePage { get; set; } = "";    
  public string ReasonPhrase { get; set; } = "";
  public string OriginalPath { get; set; } = "";
}
