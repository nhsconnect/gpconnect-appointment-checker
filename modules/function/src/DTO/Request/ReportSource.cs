﻿using GpConnect.AppointmentChecker.Function.DTO.Response;

namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class ReportSource : DataSource
{
    public OrganisationHierarchy OrganisationHierarchy { get; set; }
}
