﻿using Dapper.FluentMap.Mapping;
using GpConnect.AppointmentChecker.Api.DTO.Response.Configuration;

namespace GpConnect.AppointmentChecker.Api.DAL.Mapping;

public class EmailConfigurationMap : EntityMap<Email>
{
    public EmailConfigurationMap()
    {
        Map(p => p.SenderAddress).ToColumn("sender_address");
        Map(p => p.HostName).ToColumn("host_name");
        Map(p => p.Port).ToColumn("port");
        Map(p => p.Encryption).ToColumn("encryption");
        Map(p => p.UserName).ToColumn("user_name");
        Map(p => p.Password).ToColumn("password");
        Map(p => p.DefaultSubject).ToColumn("default_subject");
    }
}