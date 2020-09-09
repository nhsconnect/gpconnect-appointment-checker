namespace gpconnect_appointment_checker.ViewModels.Search
{
    public class SearchResultItem
    {
        public string AppointmentDate { get; set; }
        public string Location { get; set; }
        public string SessionName { get; set; }
        public string StartTime { get; set; }
        public string Duration { get; set; }
        public string SlotType { get; set; }
        public string DeliveryChannel { get; set; }
        public string Practitioner { get; set; }
        public string PractitionerRole { get; set; }
        public string PractitionerGender { get; set; }
    }
}