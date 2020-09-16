namespace gpconnect_appointment_checker.DAL.Interfaces
{
    public interface IAuditService
    {
        void AddEntry(DTO.Request.Audit.Entry auditEntry);
    }
}
