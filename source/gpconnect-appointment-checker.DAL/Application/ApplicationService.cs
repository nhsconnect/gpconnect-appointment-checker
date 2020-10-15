using Dapper;
using gpconnect_appointment_checker.DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.DAL.Application
{
    public class ApplicationService : IApplicationService
    {
        private readonly ILogger<ApplicationService> _logger;
        private readonly IDataService _dataService;
        private readonly IAuditService _auditService;

        public ApplicationService(IConfiguration configuration, ILogger<ApplicationService> logger, IDataService dataService, IAuditService auditService)
        {
            _logger = logger;
            _dataService = dataService;
            _auditService = auditService;
        }

        public async Task<DTO.Response.Application.Organisation> GetOrganisation(string odsCode)
        {
            var functionName = "application.get_organisation";
            var parameters = new DynamicParameters();
            parameters.Add("_ods_code", odsCode, DbType.String, ParameterDirection.Input);
            var result = await _dataService.ExecuteFunction<DTO.Response.Application.Organisation>(functionName, parameters);
            return result.FirstOrDefault();
        }

        public async void SynchroniseOrganisation(DTO.Response.Application.Organisation organisation)
        {
            var functionName = "application.synchronise_organisation";
            var parameters = new DynamicParameters();
            parameters.Add("_ods_code", organisation.ODSCode);
            parameters.Add("_organisation_type_name", organisation.OrganisationTypeCode);
            parameters.Add("_organisation_name", organisation.OrganisationName);
            parameters.Add("_address_line_1", organisation.PostalAddress);
            parameters.Add("_address_line_2", string.Empty);
            parameters.Add("_locality", string.Empty);
            parameters.Add("_city", string.Empty);
            parameters.Add("_county", string.Empty);
            parameters.Add("_postcode", organisation.PostalCode);
            await _dataService.ExecuteFunction(functionName, parameters);
        }
        public async Task<DTO.Response.Application.User> LogonUser(DTO.Request.Application.User user)
        {
            var functionName = "application.logon_user";
            var parameters = new DynamicParameters();
            parameters.Add("_email_address", user.EmailAddress);
            parameters.Add("_display_name", user.DisplayName);
            parameters.Add("_organisation_id", user.OrganisationId);
            var result = await _dataService.ExecuteFunction<DTO.Response.Application.User>(functionName, parameters);
            return result.FirstOrDefault();
        }

        public async Task<DTO.Response.Application.User> LogoffUser(DTO.Request.Application.User user)
        {
            var functionName = "application.logoff_user";
            var parameters = new DynamicParameters();
            parameters.Add("_email_address", user.EmailAddress);
            parameters.Add("_user_session_id", user.UserSessionId);
            var result = await _dataService.ExecuteFunction<DTO.Response.Application.User>(functionName, parameters);
            return result.FirstOrDefault();
        }

        public async void SetUserAuthorised(DTO.Request.Application.User user)
        {
            var functionName = "application.set_user_isauthorised";
            var parameters = new DynamicParameters();
            parameters.Add("_email_address", user.EmailAddress);
            parameters.Add("_is_authorised", user.IsAuthorised);
            await _dataService.ExecuteFunction(functionName, parameters);
        }
    }
}
