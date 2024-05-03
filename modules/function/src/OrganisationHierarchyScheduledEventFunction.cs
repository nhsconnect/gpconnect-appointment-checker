using Amazon.Lambda.Core;
using CsvHelper;
using GpConnect.AppointmentChecker.Function.Configuration;
using GpConnect.AppointmentChecker.Function.DTO.Request;
using GpConnect.AppointmentChecker.Function.Helpers;
using GpConnect.AppointmentChecker.Function.Helpers.Constants;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace GpConnect.AppointmentChecker.Function;

public class OrganisationHierarchyScheduledEventFunction
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _options;
    private readonly SecretManager _secretManager;
    private readonly EndUserConfiguration _endUserConfiguration;
    private readonly StorageConfiguration _storageConfiguration;
    private ILambdaContext _lambdaContext;

    public OrganisationHierarchyScheduledEventFunction()
    {
        _secretManager = new SecretManager();
        _endUserConfiguration = JsonConvert.DeserializeObject<EndUserConfiguration>(_secretManager.Get("enduser-configuration"));
        _storageConfiguration = JsonConvert.DeserializeObject<StorageConfiguration>(_secretManager.Get("storage-configuration"));

        var apiUrl = _endUserConfiguration?.ApiBaseUrl ?? throw new ArgumentNullException("ApiBaseUrl");

        _httpClient = new HttpClient
        {
            BaseAddress = new UriBuilder(apiUrl).Uri,
            Timeout = TimeSpan.FromMinutes(15)
        };

        _options = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task<HttpStatusCode> FunctionHandler(ILambdaContext lambdaContext)
    {
        _lambdaContext = lambdaContext;
        await Reset(Objects.Key, Objects.Transient);
        _lambdaContext.Logger.LogLine("START: Loading DataSource");
        var codesSuppliers = await LoadDataSource();
        _lambdaContext.Logger.LogLine("FINISH: Loading DataSource");

        _lambdaContext.Logger.LogLine("START: Loading Roles");
        var rolesSource = await StorageManager.Get<List<string>>(new StorageDownloadRequest { BucketName = _storageConfiguration.BucketName, Key = _storageConfiguration.RolesObject });
        _lambdaContext.Logger.LogLine("FINISH: Loading Roles");

        _lambdaContext.Logger.LogLine("START: Loading Ods Data");
        var odsList = await GetOdsData(rolesSource);
        _lambdaContext.Logger.LogLine("FINISH: Loading Ods Data");

        _lambdaContext.Logger.LogLine("START: Setting DataSource");
        var dataSource = codesSuppliers.Where(x => odsList.Contains(x.OdsCode)).ToList();
        _lambdaContext.Logger.LogLine("FINISH: Setting DataSource");

        _lambdaContext.Logger.LogLine("START: PersistOrganisationHierarchy");
        var hierarchyKey = await PersistOrganisationHierarchy(dataSource.DistinctBy(x => x.OdsCode).Select(x => x.OdsCode).ToList());
        _lambdaContext.Logger.LogLine("FINISH: PersistOrganisationHierarchy"); 
        ;
        return hierarchyKey != null ? HttpStatusCode.Created : HttpStatusCode.BadRequest;        
    }

    private async Task<List<DataSource>> LoadDataSource()
    {
        var reportSource = await StorageManager.Get(new StorageDownloadRequest()
        {
            BucketName = _storageConfiguration.BucketName,
            Key = _storageConfiguration.SourceObject
        });

        using var reader = new StringReader(reportSource);
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
        return csvReader.GetRecords<DataSource>().DistinctBy(x => x.OdsCode).OrderBy(x => x.OdsCode).ToList();
    }

    private async Task<string?> PersistOrganisationHierarchy(List<string> odsCodes)
    {
        var json = new StringContent(JsonConvert.SerializeObject(odsCodes, null, _options),
               Encoding.UTF8,
               MediaTypeHeaderValue.Parse("application/json").MediaType);

        _lambdaContext.Logger.Log("List<string> odsCodes in PersistOrganisationHierarchy");
        _lambdaContext.Logger.Log(await json.ReadAsStringAsync());

        var response = await _httpClient.PostWithHeadersAsync("/organisation/hierarchy", new Dictionary<string, string>()
        {
            [Headers.UserId] = _endUserConfiguration.UserId,
            [Headers.ApiKey] = _endUserConfiguration.ApiKey
        }, json);        

        if (response.IsSuccessStatusCode)
        {
            var fileStream = await response.Content.ReadAsStreamAsync();
            var byteArray = StreamExtensions.UseBufferedStream(fileStream);
            var hierarchyKey = $"{Objects.Hierarchy}_{DateTime.UtcNow.Ticks}.json".ToLower();

            await Reset(Objects.Hierarchy);
            await StorageManager.Post(new StorageUploadRequest
            {
                BucketName = _storageConfiguration.BucketName,
                InputBytes = byteArray,
                Key = hierarchyKey
            });
            return hierarchyKey;
        }
        return null;        
    }

    private async Task<string[]> GetOdsData(List<string> roles)
    {
        var queryBuilder = new QueryBuilder
        {
            { "roles", string.Join(",", roles) }
        };

        var response = await _httpClient.GetWithHeadersAsync($"/organisation/ods{queryBuilder.ToQueryString()}", new Dictionary<string, string>()
        {
            [Headers.UserId] = _endUserConfiguration.UserId,
            [Headers.ApiKey] = _endUserConfiguration.ApiKey
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<string[]>(body, _options);
    }

    private async Task Reset(params string[] objectPrefix)
    {
        foreach (var key in objectPrefix)
        {
            await StorageManager.Purge(new StorageListRequest
            {
                BucketName = _storageConfiguration.BucketName,
                ObjectPrefix = key
            });
        }
    }
}
