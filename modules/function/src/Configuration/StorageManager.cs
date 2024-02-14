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
            var request = new PutObjectRequest
            {
                BucketName = storageUploadRequest.BucketName,
                Key = storageUploadRequest.Key,
                InputStream = new MemoryStream(storageUploadRequest.InputBytes),
                AutoCloseStream = true,
                BucketKeyEnabled = true
            };
            var client = GetS3Client();
            var response = await client.PutObjectAsync(request);

            var url = GetPresignedUrl(storageUploadRequest);
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
        var preSignedUrl = client.GetPreSignedURL(new GetPreSignedUrlRequest()
        {
            BucketName = storageUploadRequest.BucketName,
            Key = storageUploadRequest.Key,
            ContentType = storageUploadRequest.ContentType,
            Expires = DateTime.UtcNow.AddHours(12)
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
