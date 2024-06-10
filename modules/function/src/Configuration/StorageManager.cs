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
            var responseBody = await GetResponseStream(storageDownloadRequest);
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

    public static async Task<string?> Get(StorageDownloadRequest storageDownloadRequest)
    {
        try
        {
            var responseBody = await GetResponseStream(storageDownloadRequest);
            return responseBody;
        }
        catch (Exception e)
        {
            Console.WriteLine("The download request has resulted in an error: " + e.Message);
            throw;
        }
    }

    private static async Task<string?> GetResponseStream(StorageDownloadRequest storageDownloadRequest)
    {
        var request = new GetObjectRequest
        {
            BucketName = storageDownloadRequest.BucketName,
            Key = storageDownloadRequest.Key
        };

        using GetObjectResponse response = await s3Client.GetObjectAsync(request);
        var responseBody = await new StreamReader(response.ResponseStream).ReadToEndAsync();
        return responseBody;
    }

    public static async Task<string?> GetObjectKey(StorageListRequest storageListRequest)
    {
        var listRequest = new ListObjectsV2Request
        {
            BucketName = storageListRequest.BucketName,
            Prefix = storageListRequest.ObjectPrefix
        };
        var s3Object = (await s3Client.ListObjectsV2Async(listRequest)).S3Objects;
        return s3Object.FirstOrDefault()?.Key;
    }

    public static async Task<List<S3Object>> GetObjects(StorageListRequest storageListRequest)
    {
        var listRequest = new ListObjectsV2Request
        {
            BucketName = storageListRequest.BucketName,
            Prefix = storageListRequest.ObjectPrefix
        };
        return (await s3Client.ListObjectsV2Async(listRequest)).S3Objects;
    }


    public static async Task<DeleteObjectsResponse?> Purge(StorageListRequest storageListRequest)
    {
        try
        {
            var listResponse = await GetObjects(storageListRequest);

            if (listResponse != null && listResponse.Count > 0)
            {
                var deleteRequest = new DeleteObjectsRequest();
                foreach (S3Object s3Object in listResponse)
                {
                    deleteRequest.BucketName = storageListRequest.BucketName;
                    deleteRequest.AddKey(s3Object.Key);
                }
                var deleteResponse = await s3Client.DeleteObjectsAsync(deleteRequest);
                return deleteResponse;
            }
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine("The purge request has resulted in an error: " + e.Message);
            throw;
        }
    }

    public static async Task<string> Post(StorageUploadRequest storageUploadRequest)
    {
        try
        {
            var url = GetPreSignedUrl(storageUploadRequest, HttpVerb.PUT);

            var httpRequest = WebRequest.Create(url) as HttpWebRequest;
            httpRequest.Method = HttpMethod.Put.Method;
            httpRequest.ContentLength = storageUploadRequest.InputBytes.Length;

            var requestStream = httpRequest.GetRequestStream();
            requestStream.Write(storageUploadRequest.InputBytes, 0, storageUploadRequest.InputBytes.Length);
            requestStream.Close();
            httpRequest.GetResponse();

            return GetPreSignedUrl(storageUploadRequest, HttpVerb.GET);
        }
        catch (Exception e)
        {
            Console.WriteLine("The upload request has resulted in an error: " + e.Message);
            throw;
        }
    }

    private static string GetPreSignedUrl(StorageUploadRequest storageUploadRequest, HttpVerb httpVerb)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = storageUploadRequest.BucketName,
            Key = storageUploadRequest.Key,
            Verb = httpVerb,
            Expires = DateTime.UtcNow.AddDays(7)
        };        

        return s3Client.GetPreSignedURL(request);
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
