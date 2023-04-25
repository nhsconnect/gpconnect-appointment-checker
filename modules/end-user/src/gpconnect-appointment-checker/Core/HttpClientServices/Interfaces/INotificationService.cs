using GpConnect.AppointmentChecker.Models.Request;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;

public interface INotificationService
{
    public Task PostNotificationAsync(NotificationDetails notificationDetails);
}
