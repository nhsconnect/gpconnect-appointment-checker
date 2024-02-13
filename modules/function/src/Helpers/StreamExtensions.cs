namespace GpConnect.AppointmentChecker.Function.Helpers;

public static class StreamExtensions
{
    public static byte[] ConvertStreamToByteArray(Stream input)
    {
        byte[] bytes;
        using (var reader = new StreamReader(input))
        {
            bytes = System.Text.Encoding.UTF8.GetBytes(reader.ReadToEnd());
        }
        input.Close();
        return bytes;
    }
}
