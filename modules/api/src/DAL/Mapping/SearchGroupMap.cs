﻿using Dapper.FluentMap.Mapping;
using GpConnect.AppointmentChecker.Api.DTO.Response.Application;

namespace GpConnect.AppointmentChecker.Api.DAL.Mapping;

public class SearchGroupMap : EntityMap<SearchGroup>
{
    public SearchGroupMap()
    {
        Map(p => p.SearchGroupId).ToColumn("search_group_id");
        Map(p => p.ProviderOdsTextbox).ToColumn("provider_ods_textbox");
        Map(p => p.ConsumerOdsTextbox).ToColumn("consumer_ods_textbox");
        Map(p => p.SelectedDateRange).ToColumn("selected_daterange");
        Map(p => p.SearchStartAt).ToColumn("search_start_at");
        Map(p => p.SearchEndAt).ToColumn("search_end_at");
        Map(p => p.ConsumerOrganisationTypeDropdown).ToColumn("consumer_organisation_type_dropdown");
    }
}
