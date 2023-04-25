namespace GpConnect.AppointmentChecker.Api.Core;

public static class WebConfigurationBuilder
{
    public static void ConfigureWebHostDefaults(IWebHostBuilder webHostDefaultsBuilder)
    {
        webHostDefaultsBuilder.UseKestrel(options =>
            {
                options.AddServerHeader = false;
            });
    }

    public static void ConfigureWebHost(IWebHostBuilder webHostBuilder)
    {
        webHostBuilder.CaptureStartupErrors(true).UseSetting("detailedErrors", "");
    }
}
