using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;

namespace gpconnect_appointment_checker.Pages
{
    public class BaseModel : PageModel
    {        
        protected static FileStreamResult GetFileStream(MemoryStream memoryStream, string fileName = null)
        {
            return new FileStreamResult(memoryStream, new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
            {
                FileDownloadName = fileName ?? $"{DateTime.UtcNow.ToFileTimeUtc()}.xlsx"
            };
        }
    }
}