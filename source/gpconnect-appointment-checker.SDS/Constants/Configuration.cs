namespace gpconnect_appointment_checker.SDS.Constants
{
    public class SearchBase
    { 
        public const string Organisation = "LdapQueries_OrganisationsSearchBase";
        public const string Services = "LdapQueries_ServicesSearchBase";
    }

    public class LdapQuery
    {
        public const string GetOrganisationDetailsByOdsCode = "LdapQueries_GetOrganisationDetailsByOdsCode";
        public const string OrganisationHasAppointmentsProviderSystemByOdsCode = "LdapQueries_OrganisationHasAppointmentsProviderSystemByOdsCode";
        public const string OrganisationHasAppointmentsConsumerSystemByOdsCode = "LdapQueries_OrganisationHasAppointmentsConsumerSystemByOdsCode";
        public const string GetGpProviderEndpointAndPartyKeyByOdsCode = "LdapQueries_GetGpProviderEndpointAndPartyKeyByOdsCode";
        public const string GetGpProviderAsIdByOdsCodeAndPartyKey = "LdapQueries_GetGpProviderAsIdByOdsCodeAndPartyKey";
    }
}
