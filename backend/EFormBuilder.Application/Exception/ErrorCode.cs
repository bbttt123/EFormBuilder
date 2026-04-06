using System.Net;

namespace EFormBuilder.Domain.Exceptions;

public class ErrorCode
{
    public string Code { get; }
    public HttpStatusCode HttpStatus { get; }
    public string DefaultMessage { get; }

    private ErrorCode(string code, HttpStatusCode httpStatus, string defaultMessage)
    {
        Code = code;
        HttpStatus = httpStatus;
        DefaultMessage = defaultMessage;
    }

    // --- Hệ thống & Chung ---
    public static readonly ErrorCode InternalError = new("SYS_001", HttpStatusCode.InternalServerError, "Lỗi hệ thống");
    public static readonly ErrorCode BadRequest = new("SYS_002", HttpStatusCode.BadRequest, "Yêu cầu không hợp lệ");

    // --- Auth & User ---
    public static readonly ErrorCode UserNotFound = new("USER_001", HttpStatusCode.NotFound, "Không tìm thấy người dùng");
    public static readonly ErrorCode EmailAlreadyExists = new("USER_002", HttpStatusCode.BadRequest, "Email này đã được sử dụng");
    public static readonly ErrorCode UserNotActive = new("USER_003", HttpStatusCode.Forbidden, "Tài khoản chưa được kích hoạt");
    public static readonly ErrorCode UserAlreadyActive = new("USER_004", HttpStatusCode.BadRequest, "Tài khoản đã được kích hoạt trước đó");

    public static readonly ErrorCode InvalidCredentials = new("AUTH_001", HttpStatusCode.Unauthorized, "Sai email hoặc mật khẩu");

    // --- OTP ---
    public static readonly ErrorCode InvalidOtp = new("OTP_001", HttpStatusCode.BadRequest, "Mã OTP không chính xác");
    public static readonly ErrorCode OtpExpired = new("OTP_002", HttpStatusCode.BadRequest, "Mã OTP đã hết hạn");
    public static readonly ErrorCode MailSendFailed = new("OTP_003", HttpStatusCode.ServiceUnavailable, "Không thể gửi email xác thực");

    // --- Form ---
    public static readonly ErrorCode FormNotFound = new("FORM_001", HttpStatusCode.NotFound, "Form không tồn tại hoặc bạn không có quyền truy cập");
    public static readonly ErrorCode InvalidFormStatus = new("FORM_002", HttpStatusCode.BadRequest, "Status không hợp lệ. Chỉ chấp nhận: Draft, Published, Closed");
    public static readonly ErrorCode SlugAlreadyExists = new("FORM_003", HttpStatusCode.BadRequest, "Slug này đã được sử dụng");
    public static readonly ErrorCode InvalidField = new("FORM_005", HttpStatusCode.BadRequest, "Không có field để Publish");
    // --- Field ---
    public static readonly ErrorCode FieldNotFound = new("FIELD_001", HttpStatusCode.NotFound, "Field không tồn tại hoặc bạn không có quyền truy cập");
    public static readonly ErrorCode InvalidFieldType = new("FIELD_002", HttpStatusCode.BadRequest, "Loại field không hợp lệ");
    public static readonly ErrorCode InvalidFieldOrder = new("FIELD_003", HttpStatusCode.BadRequest, "Danh sách sắp xếp không hợp lệ");


    // --- Public Form ---
    public static readonly ErrorCode FormNotPublished = new("FORM_004", HttpStatusCode.Forbidden, "Form chưa được công bố hoặc đã đóng");

    // --- Submit ---
    public static readonly ErrorCode DuplicateEmail = new("RESP_001", HttpStatusCode.Conflict, "Email này đã submit form rồi");
    public static readonly ErrorCode RequiredFieldMissing = new("RESP_002", HttpStatusCode.BadRequest, "Vui lòng điền đầy đủ các trường bắt buộc");
    public static readonly ErrorCode InvalidFieldReference = new("RESP_003", HttpStatusCode.BadRequest, "FieldId không thuộc form này");

    // --- Response ---
    public static readonly ErrorCode ResponseNotFound = new("RESP_004", HttpStatusCode.NotFound, "Không tìm thấy response");
}