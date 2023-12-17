using System.Net;
using System.Text.Json;
using System.Diagnostics;


namespace delivery;

public class ExceptionHandlerMiddleware
{
    // Этот Middleware перехватывает исключения, которые возникают внутри контроллера
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        try
        {
            await _next(context);
            
            // stopWatch.Stop();

            _logger.LogInformation(
                $"Class: {this}\n" +
                $"Path: {context.Request.Path}\n" + 
                $"Execution speed: stopWatch.Elapsed.Seconds:stopWatch.Elapsed.Milliseconds");
        }
        catch (Exception exception)
        {   
            _logger.LogError(
                $"Class: {this}\n" +
                $"Path: {context.Request.Path}\n" + 
                $"Execution speed: stopWatch.Elapsed.Seconds:stopWatch.Elapsed.Milliseconds\n"+
                $"Message: {exception.Message}\n" +
                $"StackTrace: {exception.StackTrace}\n");

            context.Response.ContentType = "application/json";
            string result = JsonSerializer.Serialize(
                new { result = "Internal Error",
                      status = HttpStatusCode.InternalServerError
                    }
                );
            await context.Response.WriteAsync(result);
        }
    }
}