using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toggles.AspNetCore
{
    public class TogglesMiddleware 
    {
        private readonly RequestDelegate _next;

        public TogglesMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(HttpContext context, IToggle toggles)
        {
            var text = toggles
                .CurrentStates
                .OrderBy(x => x.Key)
                .Aggregate("", (acc, pair) => acc += $"{pair.Key}:{pair.Value}\n", s => s.TrimEnd('\n'));

            return context.Response.WriteAsync(text, Encoding.UTF8);
        }
    }
}
