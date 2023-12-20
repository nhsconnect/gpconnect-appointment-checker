using GpConnect.AppointmentChecker.Api.DTO.Request;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface INotificationService
{
    Task<int> PostNotificationAsync(NotificationCreateRequest notificationCreateRequest);
}
