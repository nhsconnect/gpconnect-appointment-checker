using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using GpConnect.AppointmentChecker.Function.DTO.Request;
using Newtonsoft.Json;
using System.Net;

namespace GpConnect.AppointmentChecker.Function.Configuration;

public static class StorageManager
{
    private static AmazonS3Client s3Client = GetS3Client();

    public static async Task<T?> Get<T>(StorageDownloadRequest storageDownloadRequest) where T : class
    {
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = storageDownloadRequest.BucketName,
                Key = storageDownloadRequest.Key
            };
            using GetObjectResponse response = await s3Client.GetObjectAsync(request);
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

    public static async Task<HttpStatusCode> Post(StorageUploadRequest storageUploadRequest)
    {
        try
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = storageUploadRequest.BucketName,
                Key = storageUploadRequest.Key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            var url = s3Client.GetPreSignedURL(request);

            var httpRequest = WebRequest.Create(url) as HttpWebRequest;
            httpRequest.Method = HttpMethod.Put.Method;
            httpRequest.ContentLength = storageUploadRequest.InputBytes.Length;

            var requestStream = httpRequest.GetRequestStream();
            requestStream.Write(storageUploadRequest.InputBytes, 0, storageUploadRequest.InputBytes.Length);
            requestStream.Close();

            var httpResponse = httpRequest.GetResponse() as HttpWebResponse;

            return httpResponse.StatusCode;

            //var request = new PutObjectRequest
            //{
            //    BucketName = storageUploadRequest.BucketName,
            //    Key = storageUploadRequest.Key,
            //    InputStream = new MemoryStream(storageUploadRequest.InputBytes),
            //    AutoCloseStream = true,
            //    BucketKeyEnabled = true,                
            //    ServerSideEncryptionMethod = ServerSideEncryptionMethod.AWSKMS,
            //    TagSet = new List<Tag>() 
            //    { 
            //        new() { 
            //            Key = $"{storageUploadRequest.BucketName}-object-tag", 
            //            Value = DateTime.UtcNow.Ticks.ToString()
            //        } 
            //    }
            //};           

            //var response = await s3Client.PutObjectAsync(request);
            //var url = GetPresignedUrl(storageUploadRequest);
            //return url;
        }
        catch (Exception e)
        {
            Console.WriteLine("The upload request has resulted in an error: " + e.Message);
            throw;
        }
    }

    private static string GetPresignedUrl(StorageUploadRequest storageUploadRequest)
    {
        var request = new GetPreSignedUrlRequest()
        {
            BucketName = storageUploadRequest.BucketName,
            Key = storageUploadRequest.Key,
            ContentType = storageUploadRequest.ContentType,
            Expires = DateTime.UtcNow.AddHours(12),
            Verb = HttpVerb.GET,
            Protocol = Protocol.HTTPS,
            ServerSideEncryptionMethod = ServerSideEncryptionMethod.AWSKMS
        };
        var preSignedUrl = s3Client.GetPreSignedURL(request);
        return preSignedUrl;
    }

    private static AmazonS3Client GetS3Client()
    {
        var config = new AmazonS3Config { 
            RegionEndpoint = RegionEndpoint.EUWest2            
        };
        var client = new AmazonS3Client(config);
        return client;
    }
}
