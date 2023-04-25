using GpConnect.AppointmentChecker.Models;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;

public interface ISpineService
{
    Task<OrganisationSpine> GetOrganisation(string odsCode);
    Task<Spine> GetProviderDetails(string odsCode);
    Task<Spine> GetConsumerDetails(string odsCode);
}