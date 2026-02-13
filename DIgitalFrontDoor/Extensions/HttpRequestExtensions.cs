namespace DIgitalFrontDoor.Extensions;

public static class HttpRequestExtensions
{
    /// <summary>
    /// Reads the HTTP request body as a string asynchronously.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>The request body content as a string.</returns>
    public static async Task<string> ReadBodyAsStringAsync(this HttpRequest request)
    {
        using var reader = new StreamReader(request.Body);
        return await reader.ReadToEndAsync();
    }
}

