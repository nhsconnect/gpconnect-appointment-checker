using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace gpconnect_appointment_checker.DTO.Response.GpConnect
{
    public class CapabilityStatement
    {
        [JsonProperty("resourceType")] 
        public string ResourceType { get; set; }
        [JsonProperty("version")] 
        public string Version { get; set; }
        [JsonProperty("name")] 
        public string Name { get; set; }
        [JsonProperty("Status")]
        public string status { get; set; }
        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("publisher")]
        public string Publisher { get; set; }
        [JsonProperty("contact")]
        public List<Contact> Contact { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("copyright")]
        public string Copyright { get; set; }
        [JsonProperty("kind")]
        public string Kind { get; set; }
        [JsonProperty("software")]
        public Software Software { get; set; }
        [JsonProperty("fhirVersion")]
        public string FhirVersion { get; set; }
        [JsonProperty("acceptUnknown")]
        public string AcceptUnknown { get; set; }
        [JsonProperty("format")]
        public List<string> Format { get; set; }
        [JsonProperty("profile")]
        public List<Profile> Profile { get; set; }
        [JsonProperty("rest")]
        public List<Rest> Rest { get; set; }

        [JsonProperty("issue")]
        public List<Issue> Issue { get; set; }
    }

    public class Contact
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Software
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("releaseDate")]
        public string ReleaseDate { get; set; }
    }

    public class Profile
    {
        [JsonProperty("Reference")]
        public string reference { get; set; }
    }

    public class Security
    {
        [JsonProperty("cors")]
        public bool Cors { get; set; }
    }

    public class Interaction
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }

    public class SearchParam
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("documentation")]
        public string Documentation { get; set; }
    }

    public class CapabilityResource
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("interaction")]
        public List<Interaction> Interaction { get; set; }
        [JsonProperty("searchParam")]
        public List<SearchParam> SearchParam { get; set; }
        [JsonProperty("updateCreate")]
        public bool? UpdateCreate { get; set; }
        [JsonProperty("searchInclude")]
        public List<string> SearchInclude { get; set; }
    }

    public class Definition
    {
        [JsonProperty("reference")]
        public string Reference { get; set; }
    }

    public class Operation
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("definition")]
        public Definition Definition { get; set; }
    }

    public class Rest
    {
        [JsonProperty("mode")]
        public string Mode { get; set; }
        [JsonProperty("security")]
        public Security Security { get; set; }
        [JsonProperty("resource")]
        public List<CapabilityResource> Resource { get; set; }
        [JsonProperty("operation")]
        public List<Operation> Operation { get; set; }
    }
}
