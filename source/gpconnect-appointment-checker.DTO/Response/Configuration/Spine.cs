namespace gpconnect_appointment_checker.DTO.Response.Configuration
{
    public class Spine
    {
        public bool SingleRowLock { get; set; }
        public bool UseSSP { get; set; }
        public string SSPHostName { get; set; }
        public string SDSHostName { get; set; }
        public int SDSPort { get; set; }
        public bool SDSUseLdaps { get; set; }
        public int OrganisationId { get; set; }
        public string PartyKey { get; set; }
        public string AsId { get; set; }
    }
}
