using System.Collections.Generic;

namespace gpconnect_appointment_checker.DTO.Response.GpConnect
{
    public class SlotEntrySummary
    {
        public string LocationName { get; set; }
        public List<string> LocationAddressLines { get; set; }
        public string LocationCity { get; set; }
        public string LocationDistrict { get; set; }
        public string LocationPostalCode { get; set; }
        public string LocationCountry { get; set; }

        public bool NoAddressProvided => (LocationAddressLines == null || LocationAddressLines?.Count == 0)
                                         && string.IsNullOrEmpty(LocationCity) &&
                                         string.IsNullOrEmpty(LocationDistrict) &&
                                         string.IsNullOrEmpty(LocationPostalCode) &&
                                         string.IsNullOrEmpty(LocationCountry);
    }
}
