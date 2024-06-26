﻿namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class MessagingRequest
{
    public List<DataSource> DataSource { get; set; }
    public List<string>? Interaction { get; set; }
    public List<string>? Workflow { get; set; }
    public string? ReportName { get; set; } = null;
    public string? ReportId { get; set; } = null;
    public Guid MessageGroupId { get; set; }
}
