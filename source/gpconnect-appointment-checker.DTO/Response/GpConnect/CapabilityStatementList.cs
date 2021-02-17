namespace gpconnect_appointment_checker.DTO.Response.GpConnect
{
    public class CapabilityStatementList
    {
        public string OdsCode { get; set; }
        public CapabilityStatement CapabilityStatement { get; set; }
        public bool CapabilityStatementOk => (CapabilityStatement.Issue?.Count == 0 || CapabilityStatement.Issue == null);
    }
}
