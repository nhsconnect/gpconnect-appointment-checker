using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.DTO.Response.GpConnect
{
    public class Slot
    {
        [JsonProperty("resourceType")]
        public string ResourceType { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("entry")]
        public List<Entry> Entry { get; set; }
    }
    public class Meta
    {
        [JsonProperty("versionId")]
        public string VersionId { get; set; }
        [JsonProperty("profile")]
        public List<string> Profile { get; set; }
    }

    public class Coding
    {
        [JsonProperty("system")]
        public string System { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("display")]
        public string Display { get; set; }
    }

    public class ValueCodeableConcept
    {
        [JsonProperty("coding")]
        public List<Coding> Coding { get; set; }
    }

    public class Extension
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("valueCode")]
        public string ValueCode { get; set; }
        [JsonProperty("valueCodeableConcept")]
        public ValueCodeableConcept ValueCodeableConcept { get; set; }
    }

    public class ServiceType
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class Schedule
    {
        [JsonProperty("reference")]
        public string Reference { get; set; }
    }

    public class ServiceCategory
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class Actor
    {
        [JsonProperty("reference")]
        public string Reference { get; set; }
    }

    public class PlanningHorizon
    {
        [JsonProperty("start")]
        public DateTime Start { get; set; }
        [JsonProperty("end")]
        public DateTime End { get; set; }
    }

    public class Identifier
    {
        [JsonProperty("system")]
        public string System { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class Address
    {
        [JsonProperty("line")]
        public List<string> Line { get; set; }
        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("district")]
        public string District { get; set; }
    }

    public class Telecom
    {
        [JsonProperty("system")]
        public string System { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("use")]
        public string Use { get; set; }
    }

    public class ManagingOrganization
    {
        [JsonProperty("reference")]
        public string Reference { get; set; }
    }

    public class SlotResource
    {
        [JsonProperty("resourceType")]
        public string ResourceType { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("meta")]
        public Meta Meta { get; set; }
        [JsonProperty("extension")]
        public List<Extension> Extension { get; set; }
        [JsonProperty("serviceType")]
        public List<ServiceType> ServiceType { get; set; }
        [JsonProperty("schedule")]
        public Schedule Schedule { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("start")]
        public DateTime Start { get; set; }
        [JsonProperty("end")]
        public DateTime End { get; set; }
        [JsonProperty("serviceCategory")]
        public ServiceCategory ServiceCategory { get; set; }
        [JsonProperty("actor")]
        public List<Actor> Actor { get; set; }
        [JsonProperty("planningHorizon")]
        public PlanningHorizon PlanningHorizon { get; set; }
        [JsonProperty("identifier")]
        public List<Identifier> Identifier { get; set; }
        [JsonProperty("name")]
        public ResourceName Name { get; set; }
        [JsonProperty("gender")]
        public string Gender { get; set; }
        [JsonProperty("address")]
        public Address Address { get; set; }
        [JsonProperty("telecom")]
        public Telecom Telecom { get; set; }
        [JsonProperty("managingOrganization")]
        public ManagingOrganization ManagingOrganization { get; set; }
    }

    public class ResourceName
    {
        [JsonProperty("family")]
        public string Family { get; set; }
        [JsonProperty("given")]
        public List<string> Given { get; set; }
        [JsonProperty("prefix")]
        public List<string> Prefix { get; set; }
        [JsonProperty("gender")]
        public string Gender { get; set; }
    }

    public class Entry
    {
        [JsonProperty("resource")]
        public SlotResource Resource { get; set; }
    }
}
