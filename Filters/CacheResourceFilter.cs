using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiTestBook.Filters
{
    public class CacheResourceFilter: IResourceFilter
    {

        private static Dictionary<string, object> _cache = new();

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var key = context.HttpContext.Request.Path.ToString();

            if (_cache.TryGetValue(key, out var cachedValue))
            {
                context.Result = new ObjectResult(cachedValue); // short-circuit
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            var key = context.HttpContext.Request.Path.ToString();

            if (context.Result is ObjectResult result)
            {
                _cache[key] = result.Value?.ToString();
            }
        }
    }
}
