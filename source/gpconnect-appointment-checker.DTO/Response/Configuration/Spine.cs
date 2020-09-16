namespace gpconnect_appointment_checker.DTO.Response.Configuration
{
    public class Spine
    {
        public bool Use_SSP { get; set; }
        public string SSP_Hostname { get; set; }
        public string SDS_Hostname { get; set; }
        public int SDS_Port { get; set; }
        public bool SDS_Use_Ldaps { get; set; }
        public int Organisation_Id { get; set; }
        public string Party_Key { get; set; }
        public string AsId { get; set; }
    }
}
