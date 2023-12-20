namespace GpConnect.AppointmentChecker.Api.Helpers.Enumerations;

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
