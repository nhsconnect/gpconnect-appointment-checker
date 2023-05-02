using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices;

public class ReportingService : IReportingService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _options;

    public ReportingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _options = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task<string> GetReport(string functionName)
    {
        throw new NotImplementedException();
    }

    public async Task<string> ExportReport(string functionName, string reportName)
    {
        throw new NotImplementedException();
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
}
