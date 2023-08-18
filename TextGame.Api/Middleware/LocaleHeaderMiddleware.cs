namespace TextGame.Api.Middleware;

public class LocaleHeaderMiddleware
{
    private static readonly HashSet<string> AllowedLocales = new[]
    {
        "en-US",
        "de-DE"
    }.ToHashSet();

    private const string DefaultLocale = "en-US";

    private readonly RequestDelegate next;

    public LocaleHeaderMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var locale = context.Request.Headers.AcceptLanguage
            .Where(x => x != null)
            .FirstOrDefault(x => AllowedLocales.Contains(x!));

        context.Items["locale"] = locale ?? DefaultLocale;

        await next(context);
    }
}

public static class LocaleHeaderMiddlewareExtensions
{
    public static IApplicationBuilder UseLocaleHeader(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LocaleHeaderMiddleware>();
    }
}