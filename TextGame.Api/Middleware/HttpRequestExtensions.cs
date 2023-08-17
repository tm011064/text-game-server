using System.Text.Json;

namespace TextGame.Api.Middleware;

internal static class HttpRequestExtensions
{
    public static async Task<T> ReadBody<T>(this HttpRequest request, JsonSerializerOptions? options = null)
    {
        request.EnableBuffering();

        request.Body.Position = 0;

        var result = await request.ReadFromJsonAsync<T>(options);

        request.Body.Position = 0;

        return result!;
    }
}
