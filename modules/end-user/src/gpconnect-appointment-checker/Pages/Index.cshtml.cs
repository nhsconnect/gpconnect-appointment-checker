using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace gpconnect_appointment_checker.Pages
{
    [Authorize(Policy = "CanBeAuthorisedOrNotAuthorisedUserStatus")]
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Page();
        }
    }
}
