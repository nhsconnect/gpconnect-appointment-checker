using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace gpconnect_appointment_checker.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {            
            return Page();
        }
    }
}
