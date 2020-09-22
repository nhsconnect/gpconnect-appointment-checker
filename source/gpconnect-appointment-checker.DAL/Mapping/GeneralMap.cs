using Dapper.FluentMap.Mapping;
using gpconnect_appointment_checker.DTO.Response.Configuration;

namespace gpconnect_appointment_checker.DAL.Mapping
{
    public class GeneralMap : EntityMap<General>
    {
        public GeneralMap()
        {
            Map(p => p.MaxNumWeeksSearch).ToColumn("max_num_weeks_search");
            Map(p => p.ProductName).ToColumn("product_name");
            Map(p => p.ProductVersion).ToColumn("product_version");
        }
    }
}
