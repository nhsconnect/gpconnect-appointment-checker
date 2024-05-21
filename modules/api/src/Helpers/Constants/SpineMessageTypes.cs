using GpConnect.AppointmentChecker.Api.Core;

namespace GpConnect.AppointmentChecker.Api.Helpers.Constants;

public enum SpineMessageTypes
{
    SpineLdapQuery = 1,
    [InteractionId("urn:nhs:names:services:gpconnect:fhir:rest:read:metadata-1")]
    GpConnectReadMetaData = 2,
    GpConnectSearchFreeSlots = 3,
    SpineFhirApiSDSQuery = 4,
    SpineFhirApiOrganisationQuery = 5,
    [InteractionId("urn:nhs:names:services:gpconnect:structured:fhir:rest:read:metadata-1")]
    GpConnectReadMetaDataAccessRecordStructured = 6,
    [InteractionId("urn:nhs:names:services:gpconnect:documents:fhir:rest:read:metadata-1")]
    GpConnectReadMetaDataAccessDocument = 7,
    [InteractionId("urn:nhs:names:services:gpconnect:fhir:rest:read:metadata")]
    GpConnectReadMetaDataAccessRecordHtml = 8
}