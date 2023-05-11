using Microsoft.AspNetCore.Mvc;

namespace GpConnect.AppointmentChecker.Api.DTO.Request;

public class SearchFromDatabaseRequest
{
    [BindProperty(Name = "search_result_id", SupportsGet = true)] 
    public int SearchResultId { get; set; }
    [BindProperty(Name = "user_id", SupportsGet = true)] 
    public int UserId { get; set; }
}
