using Amazon;
using Amazon.S3;
using GpConnect.AppointmentChecker.Function.DTO.Request;

namespace GpConnect.AppointmentChecker.Function.Configuration;

public static class StorageManager
{
    private static HttpClient httpClient = new HttpClient();

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
