namespace GpConnect.AppointmentChecker.Api.DTO.Response.Logging;

public class PurgeErrorLog
{
    public int error_log_deleted_count { get; set; }
    public int spine_message_deleted_count { get; set; }
    public int web_request_deleted_count { get; set; }
}
