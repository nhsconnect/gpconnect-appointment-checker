using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models;
using GpConnect.AppointmentChecker.Models.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _options;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserService(ILogger<UserService> logger, HttpClient httpClient, IHttpContextAccessor contextAccessor, IOptions<ApplicationConfig> config)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new UriBuilder(config.Value.ApiBaseUrl).Uri;

        _contextAccessor = contextAccessor;

        _logger = logger;
        _options = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task<List<User>> GetUsersAsync(UserListSimple userListSimple)
    {
        try
        {
            var query = new Dictionary<string, string>
            {
                { "sort_by_column", userListSimple.SortByColumn.ToString() },
                { "sort_direction", userListSimple.SortDirection.ToString() }
            };
            return await GetList(query, "user/user-simple");
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, $"An exception has occurred while attempting to retrieve a list of use cases from the API");
            throw;
        }
    }

    public async Task<List<User>> GetUsersAsync(UserListAdvanced userListAdvanced)
    {
        try
        {
            var query = new Dictionary<string, string>
            {
                { "sort_by_column", userListAdvanced.SortByColumn.ToString() },
                { "sort_direction", userListAdvanced.SortDirection.ToString() },
                { "surname", userListAdvanced.Surname },
                { "email_address", userListAdvanced.EmailAddress },
                { "organisation_name", userListAdvanced.OrganisationName },
                { "user_account_status_filter", userListAdvanced.UserAccountStatusFilter.ToString() },
                { "access_level_filter", userListAdvanced.AccessLevelFilter.ToString() },
                { "multi_search_filter", userListAdvanced.MultiSearchFilter.ToString() },
                { "org_type_search_filter", userListAdvanced.OrgTypeSearchFilter.ToString() }
            };

            return await GetList(query, "user/user-advanced");
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, $"An exception has occurred while attempting to retrieve a list of use cases from the API");
            throw;
        }
    }

    private async Task<List<User>> GetList(Dictionary<string, string?> keyValuePairs, string url)
    {
        var request = QueryHelpers.AddQueryString(url, keyValuePairs);

        var response = await _httpClient.GetAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<List<User>>(content, _options);
        return result;
    }

    public async Task<User> LogonUser(LogonUser logonUser)
    {
        var json = new StringContent(
            JsonConvert.SerializeObject(logonUser, null, _options),
            Encoding.UTF8,
            MediaTypeHeaderValue.Parse("application/json").MediaType);

        var response = await _httpClient.PostAsync("/user/logonUser", json);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<User>(content, _options);
        return result;
    }

    public async Task<User> LogoffUser(LogoffUser logoffUser)
    {
        var json = new StringContent(
            JsonConvert.SerializeObject(logoffUser, null, _options),
            Encoding.UTF8,
            MediaTypeHeaderValue.Parse("application/json").MediaType);

        var response = await _httpClient.PostAsync("/user/logoffUser", json);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<User>(content, _options);
        return result;
    }

    public async Task<User> AddOrUpdateUser(UserCreateAccount userCreateAccount)
    {
        var json = new StringContent(
            JsonConvert.SerializeObject(userCreateAccount, null, _options),
            Encoding.UTF8,
            MediaTypeHeaderValue.Parse("application/json").MediaType);

        var response = await _httpClient.PostAsync("/user/addOrUpdateUser", json);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<User>(content, _options);
        return result;
    }

    public async Task<User> AddUserAsync(AddUser addUser)
    {
        var json = new StringContent(
            JsonConvert.SerializeObject(addUser, null, _options),
            Encoding.UTF8,
            MediaTypeHeaderValue.Parse("application/json").MediaType);

        var response = await _httpClient.PostAsync("/user/addUser", json);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<User>(content, _options);
        return result;
    }

    public async Task<User> GetUser(string emailAddress)
    {
        var response = await _httpClient.GetAsync($"/user/emailaddress/{emailAddress}");
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<User>(body, _options);
    }

    public async Task SetUserStatus(UserUpdateStatus userUpdateStatus)
    {
        var json = new StringContent(
            JsonConvert.SerializeObject(userUpdateStatus, null, _options),
            Encoding.UTF8,
            MediaTypeHeaderValue.Parse("application/json").MediaType);

        var response = await _httpClient.PutAsync("/user/setuserstatus", json);
        response.EnsureSuccessStatusCode();
    }

    public async Task SetMultiSearch(UserUpdateMultiSearch userUpdateMultiSearch)
    {        
        var json = new StringContent(
            JsonConvert.SerializeObject(userUpdateMultiSearch, null, _options),
            Encoding.UTF8,
            MediaTypeHeaderValue.Parse("application/json").MediaType);

        var response = await _httpClient.PutAsync("/user/setmultisearch", json);
        response.EnsureSuccessStatusCode();
    }

    public async Task SetOrgTypeSearch(UserUpdateOrgTypeSearch userUpdateOrgTypeSearch)
    {
        var json = new StringContent(
            JsonConvert.SerializeObject(userUpdateOrgTypeSearch, null, _options),
            Encoding.UTF8,
            MediaTypeHeaderValue.Parse("application/json").MediaType);

        var response = await _httpClient.PutAsync("/user/setorgtypesearch", json);
        response.EnsureSuccessStatusCode();
    }
}
