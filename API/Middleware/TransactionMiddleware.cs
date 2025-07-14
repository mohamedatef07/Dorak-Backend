using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace API
{
    public class TransactionMiddleware : IMiddleware
    {
        private readonly DorakContext context;
        private readonly ILogger<TransactionMiddleware> logger;

        public TransactionMiddleware(DorakContext context, ILogger<TransactionMiddleware> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            if (httpContext.Request.Method == HttpMethods.Get)
            {
                await next(httpContext);
                return;
            }

            IDbContextTransaction transaction = null;

            try
            {
                transaction = await context.Database.BeginTransactionAsync();

                await next(httpContext);

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                logger.LogError(ex, "An unhandled exception occurred during request execution.");
                throw;
            }
            finally
            {
                if (transaction != null)
                {
                    await transaction.DisposeAsync();
                }
            }
        }
    }
}
