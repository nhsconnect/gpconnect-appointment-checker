﻿using GpConnect.AppointmentChecker.Core.Config;
using GpConnect.AppointmentChecker.Core.HttpClientServices;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using gpconnect_appointment_checker.Configuration.Infrastructure.Authentication;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using static GpConnect.AppointmentChecker.Core.HttpClientServices.ApplicationService;
using static GpConnect.AppointmentChecker.Core.HttpClientServices.NotificationService;
using static GpConnect.AppointmentChecker.Core.HttpClientServices.SpineService;
using static GpConnect.AppointmentChecker.Core.HttpClientServices.UserService;

namespace gpconnect_appointment_checker.Configuration.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        services.AddSession(s =>
        {
            s.Cookie.Name = ".GpConnectAppointmentChecker.Session";
            s.IdleTimeout = new TimeSpan(0, 30, 0);
            s.Cookie.HttpOnly = false;
            s.Cookie.IsEssential = true;
        });

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddOptions();
        services.Configure<UserServiceConfig>(configuration.GetSection("UserApi"));
        services.Configure<SpineServiceConfig>(configuration.GetSection("SpineApi"));
        services.Configure<ApplicationServiceConfig>(configuration.GetSection("ApplicationApi"));
        services.Configure<NotificationServiceConfig>(configuration.GetSection("NotificationApi"));
        services.Configure<NotificationConfig>(configuration.GetSection("NotificationConfig"));

        services.AddHsts(options =>
        {
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(730);
        });

        services.AddScoped<ITokenService, TokenService>();

        services.AddResponseCaching();
        services.AddResponseCompression();
        services.AddHttpContextAccessor();

        services.AddHealthChecks();

        services.AddRazorPages(options =>
        {
            options.Conventions.AuthorizeFolder("/Private", "MustHaveAuthorisedUserStatus");
            options.Conventions.AuthorizeFolder("/Pending", "MustHaveNotAuthorisedUserStatus");
            options.Conventions.AuthorizeFolder("/Private/Admin", "MustHaveAuthorisedAndIsAdminUserStatus");
            options.Conventions.AllowAnonymousToFolder("/Public");
            options.Conventions.AddPageRoute("/Private/Admin/Index", "/Admin");                
            options.Conventions.AddPageRoute("/Private/Admin/Reports", "/Reports");
            options.Conventions.AddPageRoute("/Private/Search", "/Search");
            options.Conventions.AddPageRoute("/Private/SearchDetail", "/SearchDetail/{searchDetailId}");
            options.Conventions.AddPageRoute("/Public/Error", "/Error");
            options.Conventions.AddPageRoute("/Public/StatusCode", "/StatusCode/{statusCode?}");
            options.Conventions.AddPageRoute("/Public/AccessDenied", "/AccessDenied");
            options.Conventions.AddPageRoute("/Public/NotRegistered", "/NotRegistered");
            options.Conventions.AddPageRoute("/Public/Accessibility", "/Accessibility");
            options.Conventions.AddPageRoute("/Public/PrivacyAndCookies", "/PrivacyAndCookies");
            options.Conventions.AddPageRoute("/Public/TermsAndConditions", "/TermsAndConditions");
            options.Conventions.AddPageRoute("/Pending/CreateAccountInterstitial", "/CreateAccountInterstitial");
            options.Conventions.AddPageRoute("/Private/AuthorisedAccountPresent", "/AuthorisedAccountPresent");
            options.Conventions.AddPageRoute("/Pending/PendingAccount", "/PendingAccount");
            options.Conventions.AddPageRoute("/Pending/SubmitUserForm", "/SubmitUserForm");
            options.Conventions.AddPageRoute("/Pending/CreateAccount", "/CreateAccount");
            options.Conventions.AddPageRoute("/Public/Help/Help", "/Help");
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("CanBeAuthorisedOrNotAuthorisedUserStatus", policy => policy.Requirements.Add(new AuthorisedOrNotAuthorisedUserRequirement()));
            options.AddPolicy("MustHaveAuthorisedUserStatus", policy => policy.Requirements.Add(new AuthorisedUserRequirement()));                
            options.AddPolicy("MustHaveNotAuthorisedUserStatus", policy => policy.Requirements.Add(new NotAuthorisedUserRequirement()));
            options.AddPolicy("MustHaveAuthorisedAndIsAdminUserStatus", policy => policy.Requirements.Add(new AuthorisedAndIsAdminUserRequirement()));
        });

        services.AddSingleton<IAuthorizationHandler, AuthorisedOrNotAuthorisedUserHandler>();
        services.AddSingleton<IAuthorizationHandler, AuthorisedUserHandler>();
        services.AddSingleton<IAuthorizationHandler, NotAuthorisedUserHandler>();
        services.AddSingleton<IAuthorizationHandler, AuthorisedAndIsAdminUserHandler>();

        services.AddDataProtection()
            .SetApplicationName("GpConnectAppointmentChecker");

        services.AddAntiforgery(options => 
        { 
            options.SuppressXFrameOptionsHeader = true;
            options.Cookie.HttpOnly = false;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.None;
        });

        
        services.Configure<Sso>(configuration.GetSection("SingleSignOn"));
        services.Configure<General>(configuration.GetSection("General"));
        services.Configure<Spine>(configuration.GetSection("Spine"));
        services.Configure<Email>(configuration.GetSection("Email"));

        services.AddHttpClientServices(configuration, env);

        var smtpClientExtensions = new SmtpClientExtensions(configuration);
        smtpClientExtensions.AddSmtpClientServices(services);

        return services;
    }
}