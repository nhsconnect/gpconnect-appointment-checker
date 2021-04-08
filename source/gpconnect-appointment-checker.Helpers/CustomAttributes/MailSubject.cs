using System;

namespace gpconnect_appointment_checker.Helpers.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class MailSubjectAttribute : Attribute
    {
        public MailSubjectAttribute(string mailSubject)
        {
            MailSubject = mailSubject;
        }

        public string MailSubject { get; }
    }
}
