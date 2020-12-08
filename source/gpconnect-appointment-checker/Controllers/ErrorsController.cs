using gpconnect_appointment_checker.DTO.Response.Logging;
using LdapForNet;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace gpconnect_appointment_checker.Controllers
{
    public class ErrorsController : Controller
    {
        //private readonly ILogger<ErrorsController> _logger;

        //public ErrorsController(ILogger<ErrorsController> logger)
        //{
        //    _logger = logger;
        //}

        [Route("error")]
        public CustomErrorResponse Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;
            var code = 500; 

            if (exception is LdapException) code = 999;
            
            Response.StatusCode = code;
            return new CustomErrorResponse(exception);
        }
    }
}
