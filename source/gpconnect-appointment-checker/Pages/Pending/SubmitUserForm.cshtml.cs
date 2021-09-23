using Microsoft.Extensions.Configuration;

namespace gpconnect_appointment_checker.Pages
{
    public class SubmitUserFormModel : BaseModel
    {
        public SubmitUserFormModel(IConfiguration configuration) : base(configuration)
        {
        }

        public void OnGet()
        {            
        }
    }
}
