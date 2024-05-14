using GpConnect.AppointmentChecker.Api.Core.Configuration;
using GpConnect.AppointmentChecker.Api.DTO.Request.GpConnect;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace GpConnect.AppointmentChecker.Api.Service.GpConnect;

public class ReportingTokenDependencies : IReportingTokenDependencies
{
    private readonly IOptions<SpineConfig> _spineOptionsDelegate;
    private readonly IOptions<GeneralConfig> _generalOptionsDelegate;
    private readonly IUserService _userService;

    public ReportingTokenDependencies(IOptions<SpineConfig> spineOptionsDelegate, IOptions<GeneralConfig> generalOptionsDelegate, IUserService userService)
    {
        _spineOptionsDelegate = spineOptionsDelegate;
        _generalOptionsDelegate = generalOptionsDelegate;
        _userService = userService;
    }

    public void AddRequestingDeviceClaim(Uri requestUri, SecurityTokenDescriptor tokenDescriptor)
    {
        tokenDescriptor.Claims.Add("requesting_device", new RequestingDevice
        {
            resourceType = "Device",
            model = _generalOptionsDelegate.Value.ProductName,
            version = _generalOptionsDelegate.Value.ProductVersion,
            id = Guid.NewGuid().ToString(),
            identifier = new List<Identifier>
                {
                    new() {
                        system = "https://appointmentchecker.gpconnect.nhs.uk/Id/device-identifier",
                        value = requestUri.Host
                    }
                }
        });
    }

    public void AddRequestingRecordClaim(SecurityTokenDescriptor tokenDescriptor, string systemIdentifier)
    {
        tokenDescriptor.Claims.Add("requested_record", new RequestingRecord
        {
            resourceType = "Organization",
            name = _spineOptionsDelegate.Value.OrganisationName,
            identifier = new List<Identifier>
                {
                    new() {
                        system = systemIdentifier,
                        value = _spineOptionsDelegate.Value.OdsCode
                    }
                }
        });
    }

    public void AddRequestingOrganisationClaim(SecurityTokenDescriptor tokenDescriptor, string systemIdentifier)
    {
        tokenDescriptor.Claims.Add("requesting_organization", new RequestingOrganisation
        {
            resourceType = "Organization",
            name = _spineOptionsDelegate.Value.OrganisationName,
            id = Guid.NewGuid().ToString(),
            identifier = new List<Identifier>
                {
                    new() {
                        system = systemIdentifier,
                        value = _spineOptionsDelegate.Value.OdsCode
                    }
                }
        });
    }

    public SecurityTokenDescriptor BuildSecurityTokenDescriptor(string tokenIssuer, string tokenAudience, string userGuid, DateTimeOffset tokenIssuedAt, DateTimeOffset tokenExpiration)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = tokenIssuer,
            Audience = tokenAudience,
            Claims = new Dictionary<string, object>()
                {
                    {TokenRequestValues.ReasonForRequestKey, TokenRequestValues.ReasonForRequestValue},
                    {TokenRequestValues.RequestedScopeKey, TokenRequestValues.RequestedScopeValue},
                    {TokenRequestValues.TokenSubject, userGuid}
                },
            IssuedAt = tokenIssuedAt.DateTime,
            Expires = tokenExpiration.DateTime
        };
        return tokenDescriptor;
    }

    public async Task AddRequestingPractitionerClaim(Uri requestUri, SecurityTokenDescriptor tokenDescriptor, string userGuid, string Sid, string hostIdentifier, bool isID = true)
    {
        var user = await _userService.GetUserById(LoggingHelper.GetIntegerValue(Headers.UserId));
        var nameParts = Regex.Split(user.DisplayName, @"[^a-zA-Z0-9]").Where(x => x != string.Empty).ToArray();

        tokenDescriptor.Claims.Add("requesting_practitioner", new ReportingRequestingPractitioner
        {
            resourceType = "Practitioner",
            id = userGuid,
            name = new ReportingName
            {
                family = new List<string>() { StringExtensions.Coalesce(user.DisplayName, nameParts[0].FirstCharToUpper(true)) },
                given = new List<string> { StringExtensions.Coalesce(user.DisplayName, nameParts[1].FirstCharToUpper(true)) },
                prefix = new List<string> { user.DisplayName }
            },
            identifier = new List<Identifier>
                {
                    new() {
                        system = $"{hostIdentifier}/Id/sds-user-id",
                        value = "UNK"
                    },
                    new() {
                        system = $"{hostIdentifier}/{(isID ? "Id/" : string.Empty)}sds-role-profile-id",
                        value = "UNK"
                    },
                    new() {
                        system = "https://appointmentchecker.gpconnect.nhs.uk/Id/email-address",
                        value = user.EmailAddress,
                    },
                    new() {
                        system = "https://appointmentchecker.gpconnect.nhs.uk/Id/nhsmail-sid",
                        value = Sid
                    }
                },
            practitionerRole = new List<PractitionerRole>
            {
                new()
                {
                    role = new Role()
                    {
                        coding = new List<Coding> {
                            new() {
                                system = $"{hostIdentifier}/ValueSet/sds-job-role-name-1",
                                code = "UNK"
                            }
                        }
                    }
                }
            }
        });
    }
}
