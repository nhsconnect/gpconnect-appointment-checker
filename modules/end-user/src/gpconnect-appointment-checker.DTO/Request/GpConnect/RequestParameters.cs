namespace gpconnect_appointment_checker.DTO.Request.GpConnect
{
    public class RequestParameters
    {
        public string BearerToken { get; set; }
        public string SspFrom { get; set; }
        public string SspTo { get; set; }
        public string EndpointAddress { get; set; }
        public string SspHostname { get; set; }
        public bool UseSSP { get; set; }
        public string ProviderODSCode { get; set; }
        public string ConsumerODSCode { get; set; }
        public string InteractionId { get; set; }
        public int SpineMessageTypeId { get; set; }
        public int RequestTimeout { get; set; }
        public string GPConnectConsumerOrganisationType { get; set; }

        public string EndpointAddressWithSpineSecureProxy => AddSecureSpineProxy();

        private string AddSecureSpineProxy()
        {
            return UseSSP ? AddSspHostnameWithScheme() + "/" + EndpointAddress : EndpointAddress;
        }

        private string AddSspHostnameWithScheme()
        {
            return !SspHostname.StartsWith("https://") ? "https://" + SspHostname : SspHostname;
        }
    }
}
