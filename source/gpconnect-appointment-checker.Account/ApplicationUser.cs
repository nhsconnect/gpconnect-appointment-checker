using System;
using Microsoft.AspNetCore.Identity;

namespace gpconnect_appointment_checker.Account
{
    public class ApplicationUser
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
