using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Enumerations;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using User = gpconnect_appointment_checker.DTO.Request.Application.User;

namespace gpconnect_appointment_checker.SDS
{
    public class LdapTokenService : ILdapTokenService
    {
        private readonly ILogger<LdapTokenService> _logger;
        private readonly ISdsQueryExecutionBase _sdsQueryExecutionBase;
        private readonly IApplicationService _applicationService;

        public LdapTokenService(ILogger<LdapTokenService> logger, ISdsQueryExecutionBase sdsQueryExecutionBase, IApplicationService applicationService)
        {
            _sdsQueryExecutionBase = sdsQueryExecutionBase;
            _applicationService = applicationService;
            _logger = logger;
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
            var odsCode = context.Principal.GetClaimValue("ODS");
            var organisationDetails = _sdsQueryExecutionBase.GetOrganisationDetailsByOdsCode(odsCode).Result;

            if (organisationDetails != null)
            {
                var organisation = _applicationService.GetOrganisation(organisationDetails.OdsCode);

                if (organisation != null)
                { 
                    var emailAddress = StringExtensions.Coalesce(context.Principal.GetClaimValue("Email"), context.Principal.GetClaimValue("Email Address"));
                    var user = _applicationService.GetUser(emailAddress);

                    if (user != null)
                    {
                        switch ((UserAccountStatus)user.UserAccountStatusId)
                        {
                            case UserAccountStatus.Authorised:
                                var loggedOnUser = LogonAuthorisedUser(emailAddress, context, organisation);
                                PopulateAdditionalClaims((UserAccountStatus)user.UserAccountStatusId, loggedOnUser, emailAddress, context, organisation);
                                context.Properties.RedirectUri = GetAuthorisedRedirectUri(context.Properties.RedirectUri);
                                break;
                            case UserAccountStatus.Pending:
                                PopulateAdditionalClaims((UserAccountStatus)user.UserAccountStatusId, null, emailAddress, context, organisation);
                                context.Properties.RedirectUri = "/PendingAccount";
                                break;
                            case UserAccountStatus.Deauthorised:
                            case UserAccountStatus.RequestDenied:
                                PopulateAdditionalClaims((UserAccountStatus)user.UserAccountStatusId, null, emailAddress, context, organisation);
                                context.Properties.RedirectUri = "/SubmitUserForm";
                                break;
                        }
                    }
                    else
                    {
                        PopulateAdditionalClaims(null, null, emailAddress, context, organisation);
                        context.Properties.RedirectUri = GetAuthorisedRedirectUriForRegistration(context.Properties.RedirectUri);
                    }
                }
                else
                {
                    context.Properties.RedirectUri = "/";
                }

            }
            return Task.CompletedTask;
        }

        private string GetAuthorisedRedirectUri(string redirectUri)
        {
            return redirectUri == "/CreateAccount" ? "/AuthorisedAccountPresent" : "/Search";
        }

        private string GetAuthorisedRedirectUriForRegistration(string redirectUri)
        {
            return redirectUri == "/" ? "/NotRegistered" : "/CreateAccount";
        }

        private void PopulateAdditionalClaims(UserAccountStatus? userAccountStatus, DTO.Response.Application.User loggedOnUser, string emailAddress, TokenValidatedContext context, DTO.Response.Application.Organisation organisation)
        { 
            if (context.Principal.Identity is ClaimsIdentity identity)
            {
                identity.AddOrReplaceClaimValue("Email", emailAddress);
                identity.AddClaim(new Claim("OrganisationName", organisation.OrganisationName));
                identity.AddClaim(new Claim("OrganisationId", organisation.OrganisationId.ToString()));
                identity.AddClaim(new Claim("ProviderODSCode", organisation.OrganisationName));
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
                    identity.AddClaim(new Claim("OrgTypeSearchEnabled", loggedOnUser.OrgTypeSearchEnabled.ToString()));
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
