using Dapper.FluentMap.Mapping;
using GpConnect.AppointmentChecker.Api.DTO.Response.Application;

namespace GpConnect.AppointmentChecker.Api.DAL.Mapping;

public class AddSearchResultMap : EntityMap<AddSearchResult>
{
    public AddSearchResultMap()
    {
        Map(p => p.SearchGroupId).ToColumn("search_group_id");
        Map(p => p.SearchResultId).ToColumn("search_result_id");
    }
}
