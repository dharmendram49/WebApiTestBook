namespace WebApiTestBook.Middlewares
{
    public class UserContextMiddleware
    {
        private readonly RequestDelegate next;

        public UserContextMiddleware(
            RequestDelegate next
            )
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var userId = context.User?.FindFirst("Id")?.Value;

            if (!string.IsNullOrWhiteSpace(userId))
            {
                context.Items["UserId"] = userId;
            }

            await next(context);
        }
    }
}
