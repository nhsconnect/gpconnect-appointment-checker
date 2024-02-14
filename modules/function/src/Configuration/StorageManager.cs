using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using GpConnect.AppointmentChecker.Function.DTO.Request;
using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Function.Configuration;

public static class StorageManager
{
    private static HttpClient httpClient = new HttpClient();

    public static async Task<T?> Get<T>(StorageDownloadRequest storageDownloadRequest) where T : class
    {
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = storageDownloadRequest.BucketName,
                Key = storageDownloadRequest.Key
            };
            var client = GetS3Client();
            using GetObjectResponse response = await client.GetObjectAsync(request);
            var responseBody = await new StreamReader(response.ResponseStream).ReadToEndAsync();
            if (responseBody != null)
            {
                return JsonConvert.DeserializeObject<T>(responseBody);
            }
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine("The download request has resulted in an error: " + e.Message);
            throw;
        }
    }

    public static async Task<string> Post(StorageUploadRequest storageUploadRequest)
    {
        try
        {
            var url = GetPresignedUrl(storageUploadRequest);
            var inputStream = new MemoryStream(storageUploadRequest.InputBytes);
            var streamContent = new StreamContent(inputStream);
            var response = await httpClient.PutAsync(url, streamContent);
            response.EnsureSuccessStatusCode();
            return url;
        }
        catch (Exception e)
        {
            Console.WriteLine("The upload request has resulted in an error: " + e.Message);
            throw;
        }
    }

    private static string GetPresignedUrl(StorageUploadRequest storageUploadRequest)
    {
        var client = GetS3Client();
        var preSignedUrl = client.GetPreSignedURL(new Amazon.S3.Model.GetPreSignedUrlRequest()
        {
            BucketName = storageUploadRequest.BucketName,
            Key = storageUploadRequest.Key,
            ContentType = storageUploadRequest.ContentType,
            Expires = DateTime.UtcNow.AddDays(7),
            Verb = HttpVerb.PUT
        });
        return preSignedUrl;
    }

    private static AmazonS3Client GetS3Client()
    {
        var config = new AmazonS3Config { 
            RegionEndpoint = RegionEndpoint.EUWest2            
        };

        AWSConfigsS3.UseSignatureVersion4 = true;
        var client = new AmazonS3Client(config);
        return client;
    }
}
