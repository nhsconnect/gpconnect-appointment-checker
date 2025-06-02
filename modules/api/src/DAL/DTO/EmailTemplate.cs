namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class EmailTemplate
{
    public short EmailTemplateId { get; set; }

    public string? Subject { get; set; }

    public string? Body { get; set; }
}
