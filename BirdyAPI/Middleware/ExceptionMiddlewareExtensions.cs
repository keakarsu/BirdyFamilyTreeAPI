using BirdyAPI.Database;
using BirdyModel;
using BirdyModel.Database.Log;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BirdyAPI.Middleware
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        var loCrashLog = new Crashlog
                        {
                            application_name = "Birdy Test API",
                            exception = contextFeature.Error.Message,
                            exception_time = DateTime.Now
                        };

#if PROD
            loCrashLog.application_name = "Birdy API";
#endif

                        Crud<Crashlog>.InsertLog(loCrashLog, out _);
                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal Server Error.",
                            Detail = contextFeature.Error.Message
                        }.ToString());
                    }
                });
            });
        }
    }
}
