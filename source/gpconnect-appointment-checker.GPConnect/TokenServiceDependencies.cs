using gpconnect_appointment_checker.DTO.Request.GpConnect;
using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.GPConnect.Interfaces;
using gpconnect_appointment_checker.Helpers;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.RegularExpressions;

namespace gpconnect_appointment_checker.GPConnect
{
    public partial class TokenService : ITokenService
    {
        protected void AddRequestingPractitionerClaim(Uri requestUri, SecurityTokenDescriptor tokenDescriptor,
            string userGuid)
        {
            var familyName = _context.HttpContext.User.GetClaimValue("FamilyName");
            var givenName = _context.HttpContext.User.GetClaimValue("GivenName");
            var displayName = _context.HttpContext.User.GetClaimValue("DisplayName");
            var nameParts = Regex.Split(displayName, @"[^a-zA-Z0-9]").Where(x => x != string.Empty).ToArray();

            tokenDescriptor.Claims.Add("requesting_practitioner", new RequestingPractitioner
            {
                resourceType = "Practitioner",
                id = userGuid,
                name = new List<Name>
                {
                    new Name
                    {
                        family = StringExtensions.Coalesce(familyName, nameParts[0].FirstCharToUpper(true)),
                        given = new List<string> { StringExtensions.Coalesce(givenName, nameParts[1].FirstCharToUpper(true)) }
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
                        value = _context.HttpContext.User.GetClaimValue("Email")
                    },
                    new Identifier
                    {
                        system = "https://appointmentchecker.gpconnect.nhs.uk/Id/nhsmail-sid",
                        value = _context.HttpContext.User.GetClaimValue("sid")
                    }
                }
            });
        }

        protected void AddRequestingOrganisationClaim(Organisation organisationDetails,
            SecurityTokenDescriptor tokenDescriptor)
        {
            var odsCode = _configuration.GetSection("Spine:ods_code").Value;
            var organisationName = _configuration.GetSection("Spine:organisation_name").Value;

            tokenDescriptor.Claims.Add("requesting_organization", new RequestingOrganisation
            {
                resourceType = "Organization",
                name = organisationName,
                identifier = new List<Identifier>
                {
                    new Identifier
                    {
                        system = "https://fhir.nhs.uk/Id/ods-organization-code",
                        value = odsCode
                    }
                }
            });
        }

        protected void AddRequestingDeviceClaim(Uri requestUri, SecurityTokenDescriptor tokenDescriptor)
        {
            tokenDescriptor.Claims.Add("requesting_device", new RequestingDevice
            {
                resourceType = "Device",
                model = _configuration.GetSection("General:product_name").Value,
                version = _configuration.GetSection("General:product_version").Value,
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

        protected static SecurityTokenDescriptor BuildSecurityTokenDescriptor(string tokenIssuer, string tokenAudience,
            string userGuid, DateTimeOffset tokenIssuedAt, DateTimeOffset tokenExpiration)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = tokenIssuer,
                Audience = tokenAudience,
                Claims = new Dictionary<string, object>()
                {
                    {Constants.TokenRequestValues.ReasonForRequestKey, Constants.TokenRequestValues.ReasonForRequestValue},
                    {Constants.TokenRequestValues.RequestedScopeKey, Constants.TokenRequestValues.RequestedScopeValue},
                    {Constants.TokenRequestValues.TokenSubject, userGuid}
                },
                IssuedAt = tokenIssuedAt.DateTime,
                Expires = tokenExpiration.DateTime
            };
            return tokenDescriptor;
        }

        private static JwtSecurityToken AddTokenHeader(JwtSecurityTokenHandler tokenHandler, SecurityTokenDescriptor tokenDescriptor)
        {
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return token;
        }
    }
}
