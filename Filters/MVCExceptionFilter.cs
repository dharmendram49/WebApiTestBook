using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiTestBook.Filters
{
    public class MVCExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var error = context.Exception;

            var response = new
            {
                message = "Something went wrong",
                detail = error.Message
            };

            context.Result = new ObjectResult(response)
            {
                StatusCode = 500
            };

            context.ExceptionHandled = true; // 🔥 VERY IMPORTANT
        }
    }
}
