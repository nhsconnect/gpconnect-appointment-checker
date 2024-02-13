using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using GpConnect.AppointmentChecker.Function.DTO.Request;
using GpConnect.AppointmentChecker.Function.Helpers;

namespace GpConnect.AppointmentChecker.Function.Configuration;

public static class StorageManager
{
    public static byte[] Get(StorageDownloadRequest storageDownloadRequest)
    {
        try
        {
            var request = new TransferUtilityOpenStreamRequest
            {
                BucketName = storageDownloadRequest.BucketName,
                Key = storageDownloadRequest.Key                
            };
            var client = GetS3Client();
            var transferUtility = new TransferUtility(client);

            var downloadStream = transferUtility.OpenStream(request);
            return StreamExtensions.ConvertStreamToByteArray(downloadStream);
        }
        catch (Exception e)
        {
            Console.WriteLine("The download request has resulted in an error: " + e.Message);
            throw;
        }
    }

    public static Task<string> Post(StorageUploadRequest storageUploadRequest)
    {
        try
        {
            var request = new TransferUtilityUploadRequest
            {
                BucketName = storageUploadRequest.BucketName,
                Key = storageUploadRequest.Key,
                ContentType = storageUploadRequest.ContentType,
                InputStream = new MemoryStream(storageUploadRequest.InputBytes),
                AutoCloseStream = true                
            };

            var client = GetS3Client();
            var preSignedUrl = client.GetPreSignedURLAsync(new Amazon.S3.Model.GetPreSignedUrlRequest()
            {
                BucketName = storageUploadRequest.BucketName,
                Key = storageUploadRequest.Key,
                ContentType = storageUploadRequest.ContentType, 
                Protocol = Protocol.HTTPS,
                Verb = HttpVerb.GET,
                Expires = DateTime.Now.AddDays(7)                
            });

            var transferUtility = new TransferUtility(client);   
            transferUtility.Upload(request);

            Console.WriteLine("The signed URL is: " + preSignedUrl);
            return preSignedUrl;
        }
        catch (Exception e)
        {
            Console.WriteLine("The upload request has resulted in an error: " + e.Message);
            throw;
        }
    }

    private static AmazonS3Client GetS3Client()
    {
        var config = new AmazonS3Config { RegionEndpoint = RegionEndpoint.EUWest2 };
        var client = new AmazonS3Client(config);
        return client;
    }
}
