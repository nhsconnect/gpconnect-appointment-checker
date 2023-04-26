namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IAuditService
{
    Task AddEntry(DTO.Request.Audit.Entry auditEntry);
}
