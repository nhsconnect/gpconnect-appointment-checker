using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models;
using GpConnect.AppointmentChecker.Models.Request;
using gpconnect_appointment_checker.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices;

public class ReportingService : IReportingService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _options;

    public ReportingService(HttpClient httpClient, IOptions<ApplicationConfig> config)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new UriBuilder(config.Value.ApiBaseUrl).Uri;

        _options = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task<DataTable> GetReport(string functionName)
    {
        var response = await _httpClient.GetAsync($"/reporting/{functionName}");
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
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
        var response = await _httpClient.PostAsync("/reporting/export", json);
        var fileName = $"{reportExport.ReportName.ToLower().SearchAndReplace(new Dictionary<string, string>() { { " ", "_" } })}_{DateTime.UtcNow.ToFileTimeUtc()}.xlsx";
        return await GetFileStreamResult(response, fileName);
    }

    public async Task<List<Report>> GetReports()
    {
        var response = await _httpClient.GetAsync("/reporting/list");
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
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