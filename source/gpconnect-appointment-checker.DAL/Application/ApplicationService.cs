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

        public ApplicationService(IConfiguration configuration, ILogger<ApplicationService> logger, IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        public DTO.Response.Application.Organisation GetOrganisation(string odsCode)
        {
            var parameters = new DynamicParameters();
            parameters.Add("_ods_code", odsCode, DbType.String, ParameterDirection.Input);
            var result = _dataService.ExecuteFunction<DTO.Response.Application.Organisation>(Constants.Schemas.Application, Constants.Functions.GetOrganisation, parameters);
            return result.FirstOrDefault();
        }

        public void SynchroniseOrganisation(DTO.Response.Application.Organisation organisation)
        {
            var parameters = new DynamicParameters();
            parameters.Add("_ods_code", organisation.ODSCode, DbType.String, ParameterDirection.Input);
            parameters.Add("_organisation_type_name", organisation.OrganisationTypeCode, DbType.String, ParameterDirection.Input);
            parameters.Add("_organisation_name", organisation.OrganisationName, DbType.String, ParameterDirection.Input);
            parameters.Add("_address_line_1", organisation.PostalAddressFields[0], DbType.String, ParameterDirection.Input);
            parameters.Add("_address_line_2", organisation.PostalAddressFields[1], DbType.String, ParameterDirection.Input);
            parameters.Add("_locality", organisation.PostalAddressFields[2], DbType.String, ParameterDirection.Input);
            parameters.Add("_city", organisation.PostalAddressFields[3], DbType.String, ParameterDirection.Input);
            parameters.Add("_county", organisation.PostalAddressFields.Length > 4 ? organisation.PostalAddressFields[4] : string.Empty, DbType.String, ParameterDirection.Input);
            parameters.Add("_postcode", organisation.PostalCode, DbType.String, ParameterDirection.Input);
            _dataService.ExecuteFunction(Constants.Schemas.Application, Constants.Functions.SynchroniseOrganisation, parameters);
        }
        public DTO.Response.Application.User LogonUser(DTO.Request.Application.User user)
        {
            var parameters = new DynamicParameters();
            parameters.Add("_email_address", user.EmailAddress, DbType.String, ParameterDirection.Input);
            parameters.Add("_display_name", user.DisplayName, DbType.String, ParameterDirection.Input);
            parameters.Add("_organisation_id", user.OrganisationId, DbType.Int32, ParameterDirection.Input);
            var result = _dataService.ExecuteFunction<DTO.Response.Application.User>(Constants.Schemas.Application, Constants.Functions.LogonUser, parameters);
            return result.FirstOrDefault();
        }

        public DTO.Response.Application.User LogoffUser(DTO.Request.Application.User user)
        {
            var parameters = new DynamicParameters();
            parameters.Add("_email_address", user.EmailAddress, DbType.String, ParameterDirection.Input);
            parameters.Add("_user_session_id", user.UserSessionId, DbType.Int32, ParameterDirection.Input);
            var result = _dataService.ExecuteFunction<DTO.Response.Application.User>(Constants.Schemas.Application, Constants.Functions.LogoffUser, parameters);
            return result.FirstOrDefault();
        }
    }
}
