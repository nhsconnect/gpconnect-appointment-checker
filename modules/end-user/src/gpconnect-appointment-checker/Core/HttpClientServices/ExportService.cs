using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models.Request;
using gpconnect_appointment_checker.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices;

public class ExportService : IExportService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _options;

    public ExportService(HttpClient httpClient, IOptions<ApplicationConfig> config)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new UriBuilder(config.Value.ApiBaseUrl).Uri;

        _options = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task<FileStreamResult> ExportSearchResultFromDatabase(SearchExport searchExport)
    {
        var json = new StringContent(
            JsonConvert.SerializeObject(searchExport, null, _options),
            Encoding.UTF8,
            MediaTypeHeaderValue.Parse("application/json").MediaType);
        var response = await _httpClient.PostAsync("/export/exportsearchresult", json);
        var fileName = $"{searchExport.ReportName.ToLower().SearchAndReplace(new Dictionary<string, string>() { { " ", "_" } })}_{DateTime.UtcNow.ToFileTimeUtc()}.xlsx";
        return await GetFileStreamResult(response, fileName);
    }

    public async Task<FileStreamResult> ExportSearchGroupFromDatabase(SearchExport searchExport)
    {
        var json = new StringContent(
            JsonConvert.SerializeObject(searchExport, null, _options),
            Encoding.UTF8,
            MediaTypeHeaderValue.Parse("application/json").MediaType);
        var response = await _httpClient.PostAsync("/export/exportsearchgroup", json);
        var fileName = $"{searchExport.ReportName.ToLower().SearchAndReplace(new Dictionary<string, string>() { { " ", "_" } })}_{DateTime.UtcNow.ToFileTimeUtc()}.xlsx";
        return await GetFileStreamResult(response, fileName);
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
