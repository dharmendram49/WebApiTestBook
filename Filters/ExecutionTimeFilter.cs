using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace WebApiTestBook.Filters
{
    public class ExecutionTimeFilter : IActionFilter
    {

        private Stopwatch _watch;
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _watch = Stopwatch.StartNew();
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _watch.Stop();
            Console.WriteLine($"Execution Time: {_watch.ElapsedMilliseconds} ms");
        }

    }
}
