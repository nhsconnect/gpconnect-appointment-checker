namespace gpconnect_appointment_checker.SDS.Enumerations
{
    public enum FhirQueryTypes
    {
        GetOrganisationDetailsByOdsCode,
        GetAccreditedSystemDetailsFromSDS,
        GetRoutingReliabilityDetailsFromSDS,
        GetAccreditedSystemDetailsForConsumerFromSDS
    }

    public enum LdapQueryTypes
    {
        GetOrganisationDetailsByOdsCode,
        GetGpProviderEndpointAndPartyKeyByOdsCode,
        GetGpProviderAsIdByOdsCodeAndPartyKey,
        GetGpConsumerAsIdByOdsCode
    }
}
