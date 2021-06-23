using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApplicationCore.Exceptions;

namespace BudgetTracker.MVC.MiddleWare
{
    public class BudgetTrackerExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private ILogger<BudgetTrackerExceptionMiddleware> _logger;

        public BudgetTrackerExceptionMiddleware(RequestDelegate next, ILogger<BudgetTrackerExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // do your exception handling
                _logger.LogInformation("Some Exception happened, caught in this middleware: ");

                await HandleException(httpContext, ex);
            }


        }

        private async Task HandleException(HttpContext httpContext, Exception ex)
        {
            // get the exception, log with seri log and send email to dev team
            _logger.LogError("starting Exception logging:");
            switch (ex)
            {
                case ConflictException conflictException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    break;
                case NotFoundException notFoundException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case Exception exception:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }
            var errorResponse = new
            {
                ExceptionMessage = ex.Message,
                InnerExceptionMessage = ex.InnerException,
                ExceptionStackStrace = ex.StackTrace
            };

            _logger.LogError(errorResponse.ExceptionMessage);
            _logger.LogError("Exception happened on {0}", DateTime.Now);
            _logger.LogError("StackTrace of Exception {0}", errorResponse.ExceptionStackStrace);

            httpContext.Response.Redirect("/Home/Error");
            await Task.CompletedTask;
        }

    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class BudgetTrackerExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseMovieShopExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BudgetTrackerExceptionMiddleware>();
        }

    }

}
