using System.Text.Json.Serialization;

namespace EFormBuilder.Application.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Code { get; set; }
    public string? Message { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<ValidationError>? Errors { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Path { get; set; }
    public string? RequestId { get; set; }

    // ===== Factory methods (Thay cho @Builder và static methods bên Java) =====

    public static ApiResponse<T> SuccessResponse(T data)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data
        };
    }

    public static ApiResponse<object> Failure(string code, string message)
    {
        return new ApiResponse<object>
        {
            Success = false,
            Code = code,
            Message = message   
        };
    }
}