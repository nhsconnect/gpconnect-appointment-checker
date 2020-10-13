using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.DTO.Response.GpConnect
{
    public class SlotEntrySimple
    {
        public DateTime? AppointmentDate { get; set; }
        public string SessionName { get; set; }
        public DateTime? StartTime { get; set; }
        public double Duration { get; set; }
        public string SlotType { get; set; }
        public string DeliveryChannel { get; set; }
        public string PractitionerPrefix { get; set; }
        public string PractitionerGivenName { get; set; }
        public string PractitionerFamilyName { get; set; }
        public string PractitionerRole { get; set; }
        public string PractitionerGender { get; set; }

        public string LocationName { get; set; }
        public List<string> LocationAddressLines { get; set; }
        public string LocationCity { get; set; }
        public string LocationDistrict { get; set; }
        public string LocationPostalCode { get; set; }
        public string LocationCountry { get; set; }
    }
}
