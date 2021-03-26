namespace gpconnect_appointment_checker.DTO.Response.Configuration
{
    public class Email
    {
        public string sender_address { get; set; }
        public string host_name { get; set; }
        public int port { get; set; }
        public string encryption { get; set; }
        public bool authentication_required { get; set; }
        public string user_name { get; set; }
        public string password { get; set; }
        public string default_subject { get; set; }
    }
}
