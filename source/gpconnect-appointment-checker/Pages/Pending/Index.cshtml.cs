using Microsoft.Extensions.Configuration;

namespace gpconnect_appointment_checker.Pages
{
    public class PendingIndexModel : BaseModel
    {
        public PendingIndexModel(IConfiguration configuration) : base(configuration)
        {
        }

        public void OnGet()
        {
        }
    }
}
