using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace gpconnect_appointment_checker.DTO.Response.Fhir
{
    public class Spine
    {
        public string EndpointAddress => ExtractEndPointAddress();
        public string ManufacturingOrganisationOdsCode => ExtractValueFromManufacturingOrganisation("ManufacturingOrganisation");
        public string PartyKey => ExtractValueFromIdentifier("nhsMhsPartyKey");
        public string AsId => ExtractValueFromIdentifier("nhsSpineASID");

        [JsonProperty("entry")]
        public List<Entry> Entries { get; set; }

        public class Identifier
        {
            [JsonProperty("system")]
            public string System { get; set; }
            [JsonProperty("value")]
            public string Value { get; set; }
        }

        public class Extension
        {
            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("valueReference")]
            public ValueReference ValueReference { get; set; }
        }

        public class ValueReference
        {
            [JsonProperty("identifier")]
            public Identifier Identifier { get; set; }
        }

        public class Owner
        {
            [JsonProperty("identifier")]
            public Identifier Identifier { get; set; }
        }

        public class Endpoint
        {
            [JsonProperty("address")]
            public string Address { get; set; }
        }

        public class Resource
        {
            [JsonProperty("identifier")]
            public List<Identifier> Identifier { get; set; }

            [JsonProperty("Endpoint")]
            public Endpoint Endpoint { get; set; }

            [JsonProperty("owner")]
            public Owner Owner { get; set; }

            [JsonProperty("extension")]
            public List<Extension> Extension { get; set; }

            [JsonProperty("address")]
            public string Address { get; set; }

        }

        public class Entry
        {
            [JsonProperty("resource")]
            public Resource Resource { get; set; }
        }

        private string ExtractEndPointAddress()
        {
            if (Entries != null)
            {
                var entry = Entries.Select(x => x?.Resource?.Address).FirstOrDefault();
                return entry;
            }
            return null;
        }

        private string ExtractValueFromIdentifier(string key)
        {
            if (Entries != null)
            {
                var entry = Entries.SelectMany(x => x?.Resource?.Identifier).FirstOrDefault(x => x.System.Contains(key));
                return entry?.Value;
            }
            return null;
        }

        private string ExtractValueFromManufacturingOrganisation(string key)
        {
            if (Entries != null)
            {
                var entry = Entries.SelectMany(x => x?.Resource?.Extension).FirstOrDefault(x => x.Url.Contains(key))?.ValueReference?.Identifier;
                return entry?.Value;
            }
            return null;
        }
    }
}
