using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GpConnect.AppointmentChecker.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException();
    }

    [HttpPost]
    public async Task<IActionResult> PostNotification([FromBody] NotificationCreateRequest notificationCreateRequest)
    {
        var response = await _notificationService.PostNotificationAsync(notificationCreateRequest);
        return Ok(response);
    }
}
