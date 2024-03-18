using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Fhir;

public class Organisation
{
    [JsonProperty("link")]
    public List<Link> Links { get; set; }

    public bool HasNext => Links.Any(x => x.Relation.ToUpper() == "NEXT");

    public class Link
    {
        [JsonProperty("relation")]
        public string Relation { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }    

    [JsonProperty("entry")]
    public List<Entry> Entries { get; set; }

    public class Entry
    {
        [JsonProperty("resource")]
        public Resource Resource { get; set; }
    }

    public class Resource
    {
        [JsonProperty("id")]
        public string OdsCode { get; set; }
    }
}
