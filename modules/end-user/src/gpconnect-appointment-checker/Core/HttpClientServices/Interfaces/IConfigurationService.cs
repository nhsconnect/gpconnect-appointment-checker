using GpConnect.AppointmentChecker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;

public interface IConfigurationService
{
    Task<List<OrganisationType>> GetOrganisationTypes();
}