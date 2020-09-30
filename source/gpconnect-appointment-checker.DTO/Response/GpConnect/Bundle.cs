using System;
using System.Collections.Generic;
using System.Text;

namespace gpconnect_appointment_checker.DTO.Response.GpConnect
{
    public class RootMeta
    {
        public DateTime lastUpdated { get; set; }
        public List<string> profile { get; set; }
    }

    public class RootMeta2
    {
        public string versionId { get; set; }
        public DateTime lastUpdated { get; set; }
        public List<string> profile { get; set; }
    }

    public class RootIdentifier
    {
        public string system { get; set; }
        public string value { get; set; }
    }

    public class RootTelecom
    {
        public string system { get; set; }
        public string value { get; set; }
        public string use { get; set; }
    }

    public class RootCoding
    {
        public string system { get; set; }
        public string code { get; set; }
        public string display { get; set; }
    }

    public class RootValueCodeableConcept
    {
        public List<RootCoding> coding { get; set; }
    }

    public class RootExtension
    {
        public string url { get; set; }
        public string valueCode { get; set; }
        public RootValueCodeableConcept valueCodeableConcept { get; set; }
    }

    public class RootServiceType
    {
        public string text { get; set; }
    }

    public class RootSchedule
    {
        public string reference { get; set; }
    }

    public class RootServiceCategory
    {
        public string text { get; set; }
    }

    public class RootActor
    {
        public string reference { get; set; }
    }

    public class RootPlanningHorizon
    {
        public DateTime start { get; set; }
        public DateTime end { get; set; }
    }

    public class RootManagingOrganization
    {
        public string reference { get; set; }
    }

    public class RootResource
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public RootMeta2 meta { get; set; }
        public List<RootIdentifier> identifier { get; set; }
        public object name { get; set; }
        public string gender { get; set; }
        public List<RootTelecom> telecom { get; set; }
        public object address { get; set; }
        public List<RootExtension> extension { get; set; }
        public List<RootServiceType> serviceType { get; set; }
        public RootSchedule schedule { get; set; }
        public string status { get; set; }
        public DateTime? start { get; set; }
        public DateTime? end { get; set; }
        public RootServiceCategory serviceCategory { get; set; }
        public List<RootActor> actor { get; set; }
        public RootPlanningHorizon planningHorizon { get; set; }
        public RootManagingOrganization managingOrganization { get; set; }
    }

    public class RootEntry
    {
        public RootResource resource { get; set; }
    }

    public class Bundle
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public RootMeta meta { get; set; }
        public string type { get; set; }
        public List<RootEntry> entry { get; set; }
    }
}
