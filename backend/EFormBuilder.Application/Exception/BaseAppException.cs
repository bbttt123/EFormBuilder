namespace EFormBuilder.Domain.Exceptions;

public abstract class BaseAppException : Exception
{
    public ErrorCode ErrorCode { get; }

    // Constructor nhận ErrorCode (dùng Message mặc định của ErrorCode)
    protected BaseAppException(ErrorCode errorCode)
        : base(errorCode.DefaultMessage)
    {
        ErrorCode = errorCode;
    }

    // Constructor cho phép ghi đè Message
    public BaseAppException(ErrorCode errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    // Constructor xử lý lỗi gốc (cause) - Trong .NET gọi là InnerException
    public BaseAppException(ErrorCode errorCode, string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}