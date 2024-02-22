namespace GpConnect.AppointmentChecker.Function.Helpers;

public static class StreamExtensions
{
    public static async Task<byte[]> GetByteArray(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }
}
