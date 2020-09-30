using System.Collections.Generic;

namespace gpconnect_appointment_checker.DTO.Response.GpConnect
{    public class Practitioner
    {
        public List<PractitionerName> name { get; set; }
        public string gender { get; set; }
    }

    public class PractitionerName
    {
        public string family { get; set; }
        public List<string> given { get; set; }
        public List<string> prefix { get; set; }
    }
}
