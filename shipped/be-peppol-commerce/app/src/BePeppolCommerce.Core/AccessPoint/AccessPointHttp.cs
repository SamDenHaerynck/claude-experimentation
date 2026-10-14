using System.Text.Json;

namespace BePeppolCommerce.Core.AccessPoint;

/// <summary>
/// Transport handling shared by the provider clients: transport failures and timeouts become failed
/// results, a success response goes to <c>onSuccess</c>, anything else to <c>readErrors</c>.
/// </summary>
internal static class AccessPointHttp
{
    /// <summary>
    /// https, or plain http to a loopback host for test servers. The scheme is checked explicitly
    /// because .NET reports file: URIs (a bare "/path" on Linux) as loopback.
    /// </summary>
    public static bool IsAllowedBaseUri(Uri uri) =>
        uri.IsAbsoluteUri && (uri.Scheme == Uri.UriSchemeHttps || (uri.Scheme == Uri.UriSchemeHttp && uri.IsLoopback));

    public static async Task<AccessPointResult<T>> SendAsync<T>(
        HttpClient http,
        HttpRequestMessage request,
        Func<HttpResponseMessage, int, Task<AccessPointResult<T>>> onSuccess,
        Func<HttpResponseMessage, CancellationToken, Task<AccessPointError[]>> readErrors,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage response;
        try
        {
            response = await http.SendAsync(request, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            return AccessPointResult<T>.Fail(null, new AccessPointError("transport", ex.Message));
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            return AccessPointResult<T>.Fail(null, new AccessPointError("transport", "Timed out: " + ex.Message));
        }

        using (response)
        {
            var status = (int)response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return await onSuccess(response, status);
                }
                catch (Exception ex) when (ex is JsonException or InvalidOperationException or NotSupportedException)
                {
                    // InvalidOperationException/NotSupportedException: unsupported charset or content type.
                    return AccessPointResult<T>.Fail(status, new AccessPointError("client", "Unreadable response: " + ex.Message));
                }
            }

            return AccessPointResult<T>.Fail(status, await readErrors(response, cancellationToken));
        }
    }

    /// <summary>Reads an error body as text; null when there is none or it cannot be read.</summary>
    public static async Task<string?> ReadBodyAsync(HttpResponseMessage response, CancellationToken ct)
    {
        try
        {
            var text = await response.Content.ReadAsStringAsync(ct);
            return string.IsNullOrWhiteSpace(text) ? null : text;
        }
        catch (HttpRequestException) { return null; }
    }

    public static AccessPointError StatusError(HttpResponseMessage response) =>
        new("http", $"{(int)response.StatusCode} {response.ReasonPhrase}".Trim());
}
