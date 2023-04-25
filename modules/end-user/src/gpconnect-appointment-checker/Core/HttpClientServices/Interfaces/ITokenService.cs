using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;

public interface ITokenService
{
    Task<Task> TokenValidationAsync(TokenValidatedContext context);
}