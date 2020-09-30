using System;
using System.Collections.Generic;
using System.Text;

namespace gpconnect_appointment_checker.DTO.Response.GpConnect
{
    public class Location
    {
        public string name { get; set; }
        public LocationAddress address { get; set; }
    }

    public class LocationAddress
    {
        public List<string> line { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }
    }
}
