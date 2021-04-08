using gpconnect_appointment_checker.Helpers.CustomAttributes;

namespace gpconnect_appointment_checker.Helpers.Enumerations
{
    public enum MailTemplate
    {
        [MailSubject("GP Connect Appointment Checker - New Account Created")] 
        AuthorisedConfirmationEmail,
        [MailSubject("GP Connect Appointment Checker - Account Deactivated")]
        DeauthorisedConfirmationEmail
    }
}
