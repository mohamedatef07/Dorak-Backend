
using Dorak.DataTransferObject;

namespace API
{
    public class GlobalErrorHandlerMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalErrorHandlerMiddleware> logger;

        public GlobalErrorHandlerMiddleware(ILogger<GlobalErrorHandlerMiddleware> logger)
        {
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex) { 

                logger.LogError(ex.Message);
                ApiResponse<bool> response = new ApiResponse<bool>() {Data = false,Message ="Unexpected Error!" ,Status=500 };
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
