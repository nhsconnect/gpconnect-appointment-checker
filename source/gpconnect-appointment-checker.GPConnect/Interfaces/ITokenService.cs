using System;
using System.Threading.Tasks;
using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;

namespace gpconnect_appointment_checker.GPConnect.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateToken(Uri requestUri, Spine spineMessage, Organisation organisationDetails);
    }
}
