using Crypto_Portfolio_API.System;

namespace Crypto_Portfolio_API.Exceptions;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _requestDelegate;
    public ExceptionHandlingMiddleware(RequestDelegate requestDelegate)
    {
        _requestDelegate = requestDelegate;
        
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _requestDelegate(context);

        }
        catch (Exception ex)

        {
            await HandleGenericException(context, 500);

        }
    }

    private Task HandleGenericException(HttpContext context, int code)
    {
        Result result = new Result()
        {
            flag = true,
            code = code,
            message = "An unexpected error occurred."
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = code;
        return  context.Response.WriteAsJsonAsync(result);
        
    }
}