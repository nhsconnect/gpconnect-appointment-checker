using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using GpConnect.AppointmentChecker.Function.DTO.Request;

namespace GpConnect.AppointmentChecker.Function.Configuration;

public static class StorageManager
{
    public static Stream Get(StorageDownloadRequest storageDownloadRequest)
    {
        try
        {
            var request = new TransferUtilityOpenStreamRequest
            {
                BucketName = storageDownloadRequest.BucketName,
                Key = storageDownloadRequest.Key
            };
            var downloadStream = GetStorageInstance().OpenStream(request);
            downloadStream.Close();
            return downloadStream;
        }
        catch (Exception e)
        {
            Console.WriteLine("The download request has resulted in an error: " + e.Message);
            throw;
        }
    }

    public static void Post(StorageUploadRequest storageUploadRequest)
    {
        try
        {
            var request = new TransferUtilityUploadRequest
            {
                BucketName = storageUploadRequest.BucketName,
                Key = storageUploadRequest.Key,
                InputStream = new MemoryStream(storageUploadRequest.InputBytes),
                AutoCloseStream = true,
            };
            GetStorageInstance().Upload(request);
        }
        catch (Exception e)
        {
            Console.WriteLine("The upload request has resulted in an error: " + e.Message);
            throw;
        }
    }

    private static TransferUtility GetStorageInstance()
    {
        var config = new AmazonS3Config { RegionEndpoint = RegionEndpoint.EUWest2 };
        var client = new AmazonS3Client(config);        
        return new TransferUtility(client);
    }
}
