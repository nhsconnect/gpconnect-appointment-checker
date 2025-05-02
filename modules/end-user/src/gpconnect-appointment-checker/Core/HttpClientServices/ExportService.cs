using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models.Request;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

using gpconnect_appointment_checker.Helpers.Extensions;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices;

public class ExportService : IExportService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _options;
    private readonly IHttpContextAccessor _contextAccessor;

    public ExportService(HttpClient httpClient, IOptions<ApplicationConfig> config, IHttpContextAccessor contextAccessor)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new UriBuilder(config.Value.ApiBaseUrl).Uri;
        _contextAccessor = contextAccessor;

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
            MediaTypeNames.Application.Json);

        var response = await _httpClient.PostWithHeadersAsync("/export/exportsearchresult", new Dictionary<string, string>()
        {
            [Headers.UserId] = _contextAccessor.HttpContext?.User?.GetClaimValue(Headers.UserId)
        }, json);

        var fileName = $"{searchExport.ReportName.ToLower().SearchAndReplace(new Dictionary<string, string>() { { " ", "_" } })}_{DateTime.UtcNow.ToFileTimeUtc()}.xlsx";
        return await GetFileStreamResult(response, fileName);
    }

    public async Task<FileStreamResult> ExportSearchGroupFromDatabase(SearchExport searchExport)
    {
        var json = new StringContent(
            JsonConvert.SerializeObject(searchExport, null, _options),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);

        var response = await _httpClient.PostWithHeadersAsync("/export/exportsearchgroup", new Dictionary<string, string>()
        {
            [Headers.UserId] = _contextAccessor.HttpContext?.User?.GetClaimValue(Headers.UserId)
        }, json);

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
