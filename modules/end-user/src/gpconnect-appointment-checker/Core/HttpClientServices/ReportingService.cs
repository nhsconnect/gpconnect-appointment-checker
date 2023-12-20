using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models;
using GpConnect.AppointmentChecker.Models.Request;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices;

public class ReportingService : IReportingService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _options;
    private readonly IHttpContextAccessor _contextAccessor;

    public ReportingService(HttpClient httpClient, IOptions<ApplicationConfig> config, IHttpContextAccessor contextAccessor)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new UriBuilder(config.Value.ApiBaseUrl).Uri;
        _contextAccessor = contextAccessor;

        _options = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task<DataTable> GetReport(string functionName)
    {
        var response = await _httpClient.GetWithHeadersAsync($"/reporting/{functionName}", new Dictionary<string, string>()
        {
            [Headers.UserId] = _contextAccessor.HttpContext?.User?.GetClaimValue(Headers.UserId)
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        return body.ConvertJsonDataToDataTable();
    }

    public async Task<FileStreamResult> ExportReport(ReportExport reportExport)
    {
        var json = new StringContent(
            JsonConvert.SerializeObject(reportExport, null, _options),
            Encoding.UTF8,
            MediaTypeHeaderValue.Parse("application/json").MediaType);

        var response = await _httpClient.PostWithHeadersAsync("/reporting/export", new Dictionary<string, string>()
        {
            [Headers.UserId] = _contextAccessor.HttpContext?.User?.GetClaimValue(Headers.UserId)
        }, json);

        response.EnsureSuccessStatusCode();
        var fileName = $"{reportExport.ReportName.ToLower().SearchAndReplace(new Dictionary<string, string>() { { " ", "_" } })}_{DateTime.UtcNow.ToFileTimeUtc()}.xlsx";
        return await GetFileStreamResult(response, fileName);
    }

    public async Task<List<Report>> GetReports()
    {
        var response = await _httpClient.GetWithHeadersAsync("/reporting/list", new Dictionary<string, string>()
        {
            [Headers.UserId] = _contextAccessor.HttpContext?.User?.GetClaimValue(Headers.UserId)
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<Report>>(body, _options);
    }

    private async Task<FileStreamResult> GetFileStreamResult(HttpResponseMessage response, string fileName)
    {
        response.EnsureSuccessStatusCode();
        var ms = await response.Content.ReadAsStreamAsync();
        var fs = new FileStreamResult(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = fileName
        };
        return fs;
    }
}
