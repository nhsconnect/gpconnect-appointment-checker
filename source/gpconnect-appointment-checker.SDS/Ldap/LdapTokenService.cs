using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using gpconnect_appointment_checker.Helpers.Enumerations;
using User = gpconnect_appointment_checker.DTO.Request.Application.User;

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
            try
            {
                if (context == null) throw new ArgumentNullException(nameof(context));
                if (context.Principal == null) throw new ArgumentNullException(nameof(context.Principal));
                return PerformRedirectionBasedOnStatus(context);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "An error occurred attempting to authorise the user");
                throw;
            }
        }

        private Task PerformRedirectionBasedOnStatus(TokenValidatedContext context)
        {
            var odsCode = new List<string> { context.Principal.GetClaimValue("ODS") };
            var organisationDetails = _ldapService.GetOrganisationDetailsByOdsCode(odsCode, ErrorCode.ProviderODSCodeNotFound).FirstOrDefault();

            if (organisationDetails != null)
            {
                var organisation = _applicationService.GetOrganisation(organisationDetails.Organisation.ODSCode);
                var emailAddress = StringExtensions.Coalesce(context.Principal.GetClaimValue("Email"), context.Principal.GetClaimValue("Email Address"));
                var user = _applicationService.GetUser(emailAddress);

                if (user != null)
                {
                    switch (user.UserAccountStatus)
                    {
                        case UserAccountStatus.Authorised:
                            var loggedOnUser = LogonAuthorisedUser(emailAddress, context, organisation);
                            PopulateAdditionalClaims(user.UserAccountStatus, loggedOnUser, emailAddress, context, organisation, organisationDetails, odsCode);
                            context.Properties.RedirectUri = GetAuthorisedRedirectUri(context.Properties.RedirectUri);
                            break;
                        case UserAccountStatus.Pending:
                            PopulateAdditionalClaims(user.UserAccountStatus, null, emailAddress, context, organisation, organisationDetails, odsCode);
                            context.Properties.RedirectUri = "/PendingAccount";
                            break;
                        case UserAccountStatus.Deauthorised:
                        case UserAccountStatus.RequestDenied:
                            PopulateAdditionalClaims(user.UserAccountStatus, null, emailAddress, context, organisation, organisationDetails, odsCode);
                            context.Properties.RedirectUri = "/SubmitUserForm";
                            break;
                    }
                }
                else
                {
                    PopulateAdditionalClaims(null, null, emailAddress, context, organisation, organisationDetails, odsCode);
                    context.Properties.RedirectUri = "/SubmitUserForm";
                }
            }
            return Task.CompletedTask;
        }

        private string GetAuthorisedRedirectUri(string redirectUri)
        {
            return redirectUri == "/CreateAccount" ? "/AuthorisedAccountPresent" : "/Search";
        }

        private void PopulateAdditionalClaims(UserAccountStatus? userAccountStatus, DTO.Response.Application.User loggedOnUser, string emailAddress, TokenValidatedContext context, DTO.Response.Application.Organisation organisation, DTO.Response.Application.OrganisationList organisationDetails, List<string> odsCode)
        {
            if (context.Principal.Identity is ClaimsIdentity identity)
            {
                identity.AddOrReplaceClaimValue("Email", emailAddress);
                identity.AddClaim(new Claim("OrganisationName", organisationDetails.Organisation.OrganisationName));
                identity.AddClaim(new Claim("OrganisationId", organisation.OrganisationId.ToString()));
                identity.AddClaim(new Claim("ProviderODSCode", odsCode[0]));
                if (userAccountStatus != null)
                {
                    identity.AddClaim(new Claim("UserAccountStatus", userAccountStatus.ToString()));
                }

                if (loggedOnUser != null)
                {
                    identity.AddClaim(new Claim("UserSessionId", loggedOnUser.UserSessionId.ToString()));
                    identity.AddClaim(new Claim("UserId", loggedOnUser.UserId.ToString()));
                    identity.AddClaim(new Claim("IsAdmin", loggedOnUser.IsAdmin.ToString()));
                    identity.AddClaim(new Claim("MultiSearchEnabled", loggedOnUser.MultiSearchEnabled.ToString()));
                }
            }
        }

        private DTO.Response.Application.User LogonAuthorisedUser(string emailAddress, TokenValidatedContext context, DTO.Response.Application.Organisation organisation)
        {
            var loggedOnUser = _applicationService.LogonUser(new User
            {
                EmailAddress = emailAddress,
                DisplayName = context.Principal.GetClaimValue("DisplayName"),
                OrganisationId = organisation.OrganisationId
            });
            return loggedOnUser;
        }
    }
}
