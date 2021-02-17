using Dapper;
using gpconnect_appointment_checker.DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Linq;

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

        public DTO.Response.Application.Organisation GetOrganisation(string odsCode)
        {
            var functionName = "application.get_organisation";
            var parameters = new DynamicParameters();
            parameters.Add("_ods_code", odsCode, DbType.String, ParameterDirection.Input);
            var result = _dataService.ExecuteFunction<DTO.Response.Application.Organisation>(functionName, parameters);
            return result.FirstOrDefault();
        }

        public void SynchroniseOrganisation(DTO.Response.Application.Organisation organisation)
        {
            if (organisation != null)
            {
                var functionName = "application.synchronise_organisation";
                var parameters = new DynamicParameters();
                parameters.Add("_ods_code", organisation.ODSCode);
                parameters.Add("_organisation_type_name", organisation.OrganisationTypeCode);
                parameters.Add("_organisation_name", organisation.OrganisationName);
                parameters.Add("_address_line_1", organisation.PostalAddressFields[0]);
                parameters.Add("_address_line_2", organisation.PostalAddressFields[1]);
                parameters.Add("_locality", organisation.PostalAddressFields[2]);
                parameters.Add("_city", organisation.PostalAddressFields[3]);
                parameters.Add("_county",
                    organisation.PostalAddressFields.Length > 4 ? organisation.PostalAddressFields[4] : string.Empty);
                parameters.Add("_postcode", organisation.PostalCode);
                _dataService.ExecuteFunction(functionName, parameters);
            }
        }

        public DTO.Response.Application.User LogonUser(DTO.Request.Application.User user)
        {
            var functionName = "application.logon_user";
            var parameters = new DynamicParameters();
            parameters.Add("_email_address", user.EmailAddress);
            parameters.Add("_display_name", user.DisplayName);
            parameters.Add("_organisation_id", user.OrganisationId);
            var result = _dataService.ExecuteFunction<DTO.Response.Application.User>(functionName, parameters);
            return result.FirstOrDefault();
        }

        public DTO.Response.Application.User LogoffUser(DTO.Request.Application.User user)
        {
            var functionName = "application.logoff_user";
            var parameters = new DynamicParameters();
            parameters.Add("_email_address", user.EmailAddress);
            parameters.Add("_user_session_id", user.UserSessionId);
            var result = _dataService.ExecuteFunction<DTO.Response.Application.User>(functionName, parameters);
            return result.FirstOrDefault();
        }

        public void SetUserAuthorised(DTO.Request.Application.User user)
        {
            var functionName = "application.set_user_isauthorised";
            var parameters = new DynamicParameters();
            parameters.Add("_email_address", user.EmailAddress);
            parameters.Add("_is_authorised", user.IsAuthorised);
            _dataService.ExecuteFunction(functionName, parameters);
        }
    }
}
