using GpConnect.AppointmentChecker.Api.DTO.Request.GpConnect;
using GpConnect.AppointmentChecker.Api.DTO.Response.Application;
using GpConnect.AppointmentChecker.Api.DTO.Response.Configuration;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace GpConnect.AppointmentChecker.Api.Service.GpConnect;

public class TokenDependencies : ITokenDependencies
{
    private readonly IOptions<Spine> _spineOptionsDelegate;
    private readonly IOptions<General> _generalOptionsDelegate;

    public TokenDependencies(IOptions<Spine> spineOptionsDelegate, IOptions<General> generalOptionsDelegate)
    {
        _spineOptionsDelegate = spineOptionsDelegate;
        _generalOptionsDelegate = generalOptionsDelegate;
    }

    public void AddRequestingDeviceClaim(Uri requestUri, SecurityTokenDescriptor tokenDescriptor)
    {
        tokenDescriptor.Claims.Add("requesting_device", new RequestingDevice
        {
            resourceType = "Device",
            model = _generalOptionsDelegate.Value.ProductName,
            version = _generalOptionsDelegate.Value.ProductVersion,
            identifier = new List<Identifier>
                {
                    new Identifier
                    {
                        system = "https://appointmentchecker.gpconnect.nhs.uk/Id/device-identifier",
                        value = requestUri.Host
                    }
                }
        });
    }

    public void AddRequestingOrganisationClaim(SecurityTokenDescriptor tokenDescriptor)
    {
        tokenDescriptor.Claims.Add("requesting_organization", new RequestingOrganisation
        {
            resourceType = "Organization",
            name = _spineOptionsDelegate.Value.OrganisationName,
            identifier = new List<Identifier>
                {
                    new Identifier
                    {
                        system = "https://fhir.nhs.uk/Id/ods-organization-code",
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

    public void AddRequestingPractitionerClaim(Uri requestUri, SecurityTokenDescriptor tokenDescriptor, string userGuid, User user, string Sid)
    {
        var nameParts = Regex.Split(user.DisplayName, @"[^a-zA-Z0-9]").Where(x => x != string.Empty).ToArray();

        tokenDescriptor.Claims.Add("requesting_practitioner", new RequestingPractitioner
        {
            resourceType = "Practitioner",
            id = userGuid,
            name = new List<Name>
                {
                    new Name
                    {
                        family = StringExtensions.Coalesce(user.DisplayName, nameParts[0].FirstCharToUpper(true)),
                        given = new List<string> { StringExtensions.Coalesce(user.DisplayName, nameParts[1].FirstCharToUpper(true)) }
                    }
                },
            identifier = new List<Identifier>
                {
                    new Identifier
                    {
                        system = "https://fhir.nhs.uk/Id/sds-user-id",
                        value = "UNK"
                    },
                    new Identifier
                    {
                        system = "https://fhir.nhs.uk/Id/sds-role-profile-id",
                        value = "UNK"
                    },
                    new Identifier
                    {
                        system = "https://appointmentchecker.gpconnect.nhs.uk/Id/email-address",
                        value = user.EmailAddress,
                    },
                    new Identifier
                    {
                        system = "https://appointmentchecker.gpconnect.nhs.uk/Id/nhsmail-sid",
                        value = Sid
                    }
                }
        });
    }
}
