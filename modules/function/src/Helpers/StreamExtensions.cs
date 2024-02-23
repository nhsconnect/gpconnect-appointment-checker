namespace GpConnect.AppointmentChecker.Function.Helpers;

public static class StreamExtensions
{
    public static async Task<byte[]> GetByteArray(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

    public static byte[] UseBufferedStream(Stream stream)
    {
        byte[] bytes;
        using (var bufferedStream = new BufferedStream(stream))
        {
            using var memoryStream = new MemoryStream();
            bufferedStream.CopyTo(memoryStream);
            bytes = memoryStream.ToArray();
        }
        return bytes;
    }
}
