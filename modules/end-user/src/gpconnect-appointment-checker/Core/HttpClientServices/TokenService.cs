using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models;
using GpConnect.AppointmentChecker.Models.Request;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices;

public class TokenService : ITokenService
{
    private readonly IApplicationService _applicationService;
    private readonly ISpineService _spineService;
    private readonly IUserService _userService;
    private readonly ILogger<TokenService> _logger;

    public TokenService(ILogger<TokenService> logger, IApplicationService applicationService, ISpineService spineService, IUserService userService)
    {
        _applicationService = applicationService;
        _spineService = spineService;
        _userService = userService;
        _logger = logger;
    }

    public async Task HandleOnTokenValidatedAsync(TokenValidatedContext context)
    {
        try
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.Principal == null) throw new ArgumentNullException(nameof(context.Principal));
            await PerformRedirectionBasedOnStatus(context);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "An error occurred attempting to authorise the user");
            throw;
        }
    }

    private async Task PerformRedirectionBasedOnStatus(TokenValidatedContext context)
    {
        var odsCode = context.Principal.GetClaimValue("ODS");
        var organisationDetails = await _spineService.GetOrganisation(odsCode);

        if (organisationDetails != null)
        {
            var organisation = await _applicationService.GetOrganisationAsync(organisationDetails.OdsCode);

            if (organisation != null)
            {
                var emailAddress = StringExtensions.Coalesce(context.Principal.GetClaimValue("Email"), context.Principal.GetClaimValue("Email Address"));
                var user = await _userService.GetUser(emailAddress);

                if (user != null)
                {
                    switch ((UserAccountStatus)user.UserAccountStatusId)
                    {
                        case UserAccountStatus.Authorised:
                            var loggedOnUser = await LogonAuthorisedUser(emailAddress, context, organisation);
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
    }

    private string GetAuthorisedRedirectUri(string redirectUri)
    {
        return redirectUri == "/CreateAccount" ? "/AuthorisedAccountPresent" : "/Search";
    }

    private string GetAuthorisedRedirectUriForRegistration(string redirectUri)
    {
        return redirectUri == "/" ? "/NotRegistered" : "/CreateAccount";
    }

    private void PopulateAdditionalClaims(UserAccountStatus? userAccountStatus, User loggedOnUser, string emailAddress, TokenValidatedContext context, Organisation organisation)
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

    private async Task<User> LogonAuthorisedUser(string emailAddress, TokenValidatedContext context, Organisation organisation)
    {
        var loggedOnUser = await _userService.LogonUser(new LogonUser
        {
            EmailAddress = emailAddress,
            DisplayName = context.Principal.GetClaimValue("DisplayName"),
            OrganisationId = organisation.OrganisationId
        });
        return loggedOnUser;
    }
}
