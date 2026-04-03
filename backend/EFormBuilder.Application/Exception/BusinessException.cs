namespace EFormBuilder.Domain.Exceptions;

public class BusinessException : BaseAppException
{
    // Object? tương đương với Object bên Java (dấu ? nghĩa là có thể null)
    public object? Data { get; }

    public BusinessException(ErrorCode errorCode)
        : base(errorCode)
    {
    }

    public BusinessException(ErrorCode errorCode, object data)
        : base(errorCode)
    {
        Data = data;
    }

    public BusinessException(ErrorCode errorCode, string message)
        : base(errorCode, message)
    {
    }

    public BusinessException(ErrorCode errorCode, string message, Exception innerException)
        : base(errorCode, message, innerException)
    {
    }
}