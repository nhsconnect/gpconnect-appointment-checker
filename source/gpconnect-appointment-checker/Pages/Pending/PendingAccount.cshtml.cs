using Microsoft.Extensions.Configuration;
namespace gpconnect_appointment_checker.Pages
{
    public class PendingAccountModel : BaseModel
    {
        public PendingAccountModel(IConfiguration configuration) : base(configuration) { }

        public void OnGet()
        { 
        }
    }
}
