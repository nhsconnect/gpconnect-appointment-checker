using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Configuration.Infrastructure.ErrorHandling
{
    public static class CustomErrorHandlerHelper
    {
        public static void ErrorHandlingMiddleware(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.Use(WriteDevelopmentResponse);
            }
            else
            {
                app.Use(WriteProductionResponse);
            }
        }

        private static Task WriteDevelopmentResponse(HttpContext httpContext, Func<Task> next)
            => WriteResponse(httpContext, true);

        private static Task WriteProductionResponse(HttpContext httpContext, Func<Task> next)
            => WriteResponse(httpContext, false);

        private static async Task WriteResponse(HttpContext httpContext, bool includeDetails)
        {
            var exceptionDetails = httpContext.Features.Get<IExceptionHandlerPathFeature>();
            var ex = exceptionDetails?.Error;

            httpContext.Response.Redirect("/Error");
        }
    }
}
