using System;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using User = gpconnect_appointment_checker.DTO.Request.Application.User;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace gpconnect_appointment_checker.SDS
{
    public class LdapTokenService : ILdapTokenService
    {
        private readonly ILogger<LdapTokenService> _logger;
        private readonly ILdapService _ldapService;
        private readonly IApplicationService _applicationService;

        public LdapTokenService(IServiceProvider serviceProvider)
        {
            _ldapService = serviceProvider.GetRequiredService<ILdapService>();
            _logger = serviceProvider.GetService<ILogger<LdapTokenService>>() ?? NullLogger<LdapTokenService>.Instance;
            _applicationService = serviceProvider.GetRequiredService<IApplicationService>();
        }

        public Task ExecutionTokenValidation(TokenValidatedContext context)
        {
            ///////////////////////////
            // temporary token fix
            ///////////////////////////
            _logger.LogInformation(context.SecurityToken.RawData);
            _logger.LogInformation(context.SecurityToken.ToString());

            string emailAddress = context.Principal.GetClaimValue("Email");

            if (string.IsNullOrWhiteSpace(emailAddress))
                emailAddress = context.Principal.GetClaimValue("Email Address");
            ///////////////////////////
            // end temporary token fix
            ///////////////////////////

            var odsCode = context.Principal.GetClaimValue("ODS");
            var organisationDetails = _ldapService.GetOrganisationDetailsByOdsCode(odsCode);
            if (organisationDetails != null)
            {
                var providerGpConnectDetails = _ldapService.GetGpProviderEndpointAndPartyKeyByOdsCode(odsCode);
                var organisation = _applicationService.GetOrganisation(organisationDetails.ODSCode);
                var loggedOnUser = _applicationService.LogonUser(new User
                {
                    EmailAddress = emailAddress,
                    DisplayName = context.Principal.GetClaimValue("DisplayName"),
                    OrganisationId = organisation.OrganisationId
                });

                if (!loggedOnUser.IsAuthorised)
                {
                    context.Response.Redirect("/AccessDenied");
                    context.HandleResponse();
                }
                else
                {
                    if (context.Principal.Identity is ClaimsIdentity identity)
                    {
                        identity.AddClaim(new Claim("OrganisationName", organisationDetails.OrganisationName));
                        identity.AddClaim(new Claim("UserSessionId", loggedOnUser.UserSessionId.ToString()));
                        identity.AddClaim(new Claim("UserId", loggedOnUser.UserId.ToString()));
                        if (providerGpConnectDetails != null)
                        {
                            identity.AddClaim(new Claim("ProviderODSCode", odsCode));
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
