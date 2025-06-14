
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

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
        public async Task InvokeAsync(HttpContext httpcontext, RequestDelegate next)
        {
            if (httpcontext.Request.Method=="GET")
            {
                await next(httpcontext);
                return;
            }

            IDbContextTransaction transaction=null;
            try
            {
                transaction = await context.Database.BeginTransactionAsync();
                await next(httpcontext);

                 await transaction.CommitAsync();
            }
            catch (Exception ex) 
            {
                await transaction.RollbackAsync();
                logger.LogError(ex.Message);
                throw;

            }
        }
    }
}
