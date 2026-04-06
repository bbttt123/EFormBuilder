using System.ComponentModel.DataAnnotations;

namespace EFormBuilder.Application.DTOs.PublicForm;

public class SubmitFormRequest
{
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [MaxLength(255, ErrorMessage = "Email tối đa 255 ký tự")]
    public string? ResponderEmail { get; set; }

    [Required(ErrorMessage = "Danh sách câu trả lời không được để trống")]
    public List<AnswerRequest> Answers { get; set; } = new();
}

public class AnswerRequest
{
    [Required(ErrorMessage = "FieldId không được để trống")]
    public Guid FieldId { get; set; }

    
    public string? AnswerText { get; set; }
}

public class SubmitFormResponse
{
    public Guid ResponseId { get; set; }
    public DateTime SubmittedAt { get; set; }
}