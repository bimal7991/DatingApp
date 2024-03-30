using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate next;
        private readonly IHostEnvironment environment;

        public ExceptionMiddleware(RequestDelegate next,ILogger<ExceptionMiddleware> logger,IHostEnvironment environment)
        {
            _logger = logger;
            this.next = next;
            this.environment = environment;
        }
        public async Task InvokeAsync(HttpContext context){ 
            try {
                await next(context);
            }
            catch(Exception ex){
                _logger.LogError(ex,ex.Message);
                context.Response.ContentType="application/json";
                context.Response.StatusCode=(int)HttpStatusCode.InternalServerError;
                var response=environment.IsDevelopment() ? new APIException(context.Response.StatusCode,ex.Message,ex.StackTrace.ToString()) :
                new APIException(context.Response.StatusCode,ex.Message, "Internal Server Error") ;
                var options=new JsonSerializerOptions{PropertyNamingPolicy =JsonNamingPolicy.CamelCase};
                var json=JsonSerializer.Serialize(response,options);
                await context.Response.WriteAsync(json);
            }


        }
    }
}