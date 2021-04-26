using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace gpconnect_appointment_checker.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userAccountStatus = User.GetClaimValue<UserAccountStatus>("UserAccountStatus");
                if (userAccountStatus != UserAccountStatus.Authorised)
                {
                    return Redirect("/Pending/Index");
                }
            }
            return Page();
        }
    }
}
