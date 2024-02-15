namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public abstract class StorageRequest
{
    public string BucketName { get; set; }
    public string Key { get; set; }
}
