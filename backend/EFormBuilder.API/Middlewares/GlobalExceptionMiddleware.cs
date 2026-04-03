using System.Net;
using System.Text.Json;
using EFormBuilder.Application.DTOs;
using EFormBuilder.Domain.Exceptions;

namespace EFormBuilder.API.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var errorCode = "SYS_001";
        var message = "Internal server error";
        object? data = null;
        List<ValidationError>? validationErrors = null;

        // 1. Phân loại Exception (Tương đương nhiều @ExceptionHandler)
        switch (exception)
        {
            case BaseAppException appEx:
                statusCode = appEx.ErrorCode.HttpStatus;
                errorCode = appEx.ErrorCode.Code;
                message = appEx.Message;

                if (appEx is BusinessException bizEx)
                    data = bizEx.Data;

                if ((int)statusCode >= 500)
                    _logger.LogError(exception, "System error: {Message}", message);
                else
                    _logger.LogWarning("Business error: {Message}", message);
                break;

            case UnauthorizedAccessException: // Thường ném ra bởi hệ thống .NET
                statusCode = HttpStatusCode.Unauthorized;
                errorCode = "SEC_001";
                message = "Unauthorized access";
                break;

            // Lưu ý: Lỗi Validation trong .NET thường xử lý qua Filter (xem mục 2 bên dưới)
            // Nhưng nếu bạn throw thủ công một ValidationException thì bắt ở đây:
            case AppValidationException valEx:
                statusCode = HttpStatusCode.BadRequest;
                errorCode = "VAL_001";
                message = "Validation failed";
                validationErrors = valEx.Errors;
                break;

            default:
                _logger.LogError(exception, "Unhandled error occurred");
                break;
        }

        // 2. Đóng gói Response (Dùng đúng cấu trúc ApiResponse bạn vừa chuyển sang)
        var response = new ApiResponse<object>
        {
            Success = false,
            Code = errorCode,
            Message = message,
            Data = data,
            Errors = validationErrors,
            Timestamp = DateTime.UtcNow,
            Path = context.Request.Path,
            RequestId = context.TraceIdentifier // Tương đương X-Request-Id
        };

        // 3. Trả về JSON
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsJsonAsync(response);
    }
}