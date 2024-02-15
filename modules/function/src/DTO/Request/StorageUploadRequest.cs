namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class StorageUploadRequest : StorageRequest
{
    public byte[] InputBytes { get; set; }
    public string ObjectTagKey => $"{BucketName}-object-tag".ToLower();
    public string ObjectTagValue { get; set; }
}
