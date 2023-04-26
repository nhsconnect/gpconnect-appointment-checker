namespace GpConnect.AppointmentChecker.Api.Helpers.Constants;
public class TokenRequestValues
{
    public const string TokenHeaderAlgorithmKey = "alg";
    public const string TokenHeaderAlgorithmValue = "none";
    public const string TokenHeaderTypeKey = "typ";
    public const string TokenHeaderTypeValue = "JWT";
    public const string TokenExpiration = "exp";
    public const string IssuedAt = "iat";
    public const string TokenSubject = "sub";
    public const string ReasonForRequestKey = "reason_for_request";
    public const string ReasonForRequestValue = "directcare";
    public const string RequestedScopeKey = "requested_scope";
    public const string RequestedScopeValue = "organization/*.read";
    public const string RequestingDevice = "requesting_device";
    public const string RequestingOrganization = "requesting_organization";
    public const string RequestingPractitioner = "requesting_practitioner";
}
