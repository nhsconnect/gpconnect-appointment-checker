using gpconnect_appointment_checker.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace gpconnect_appointment_checker.Pages
{
    public class IndexModel : PageModel
    {
        public List<SelectListItem> DateRanges => GetDateRanges();

        protected IConfiguration _configuration;
        protected IHttpContextAccessor _contextAccessor;

        public IndexModel(
            IConfiguration configuration,
            IHttpContextAccessor contextAccessor)
        {
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        private List<SelectListItem> GetDateRanges()
        {
            var weeksToGet = 12;
            var dateRange = new List<SelectListItem>();
            var firstDayOfCurrentWeek = DateTime.Now.StartOfWeek(DayOfWeek.Monday);

            for (var i = 0; i < weeksToGet; i++)
            {
                var week = new SelectListItem
                {
                    Text = $"{firstDayOfCurrentWeek:d MMMM yyyy} - {firstDayOfCurrentWeek.AddDays(6):d MMMM yyyy}",
                    Value = $"{firstDayOfCurrentWeek:d-MMM-yyyy}:{firstDayOfCurrentWeek.AddDays(6):d-MMM-yyyy}"
                };
                dateRange.Add(week);
                firstDayOfCurrentWeek = firstDayOfCurrentWeek.AddDays(7);
            }
            return dateRange;
        }
    }
}
