namespace GpConnect.AppointmentChecker.Function.Helpers;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> GetWithHeadersAsync(this HttpClient httpClient, string requestUri, Dictionary<string, string> headers)
    {
        using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            return await httpClient.SendAsync(request);
        }
    }

    public static async Task<HttpResponseMessage> PostWithHeadersAsync(this HttpClient httpClient, string requestUri, Dictionary<string, string> headers, StringContent json)
    {
        return await PutOrPostWithHeadersAsync(httpClient, requestUri, headers, json, HttpMethod.Post);
    }

    public static async Task<HttpResponseMessage> PutWithHeadersAsync(this HttpClient httpClient, string requestUri, Dictionary<string, string> headers, StringContent json)
    {
        return await PutOrPostWithHeadersAsync(httpClient, requestUri, headers, json, HttpMethod.Put);
    }

    private static async Task<HttpResponseMessage> PutOrPostWithHeadersAsync(this HttpClient httpClient, string requestUri, Dictionary<string, string> headers, StringContent json, HttpMethod httpMethod)
    {
        using (var request = new HttpRequestMessage(httpMethod, requestUri))
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
            request.Content = json;
            return await httpClient.SendAsync(request);
        }
    }
}
