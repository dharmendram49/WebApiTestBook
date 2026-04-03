namespace WebApiTestBook.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<RequestLoggingMiddleware> logger;

        public RequestLoggingMiddleware(RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var startTime = System.DateTime.UtcNow;
            this.logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");

            await next(context);

            var duration = DateTime.UtcNow - startTime;

            this.logger.LogInformation($"Response Time: {duration.TotalMilliseconds} ms");

        }
    }
}
