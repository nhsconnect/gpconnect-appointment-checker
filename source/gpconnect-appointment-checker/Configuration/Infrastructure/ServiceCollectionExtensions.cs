﻿using System;
using gpconnect_appointment_checker.Configuration.Infrastructure.Authentication;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace gpconnect_appointment_checker.Configuration.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddSession(s =>
            {
                s.Cookie.Name = ".GpConnectAppointmentChecker.Session";
                s.IdleTimeout = new System.TimeSpan(0, 30, 0);
                s.Cookie.HttpOnly = false;
                s.Cookie.IsEssential = true;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddHsts(options =>
            {
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(730);
            });

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

            services.AddAntiforgery(options => 
            { 
                options.SuppressXFrameOptionsHeader = true;
                options.Cookie.HttpOnly = false;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.None;
            });
            HttpClientExtensions.AddHttpClientServices(services, configuration, env);
            SmtpClientExtensions.AddSmtpClientServices(services, configuration);
            return services;
        }
    }
}
