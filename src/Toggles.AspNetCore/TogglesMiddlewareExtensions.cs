using Microsoft.AspNetCore.Builder;

namespace Toggles.AspNetCore
{
    public static class TogglesMiddlewareExtensions
    {
        public static IApplicationBuilder UseToggles(this IApplicationBuilder app, string path = "/toggles")
        {
            return app.Map(path, builder => builder.UseMiddleware<TogglesMiddleware>());
        }
    }
}