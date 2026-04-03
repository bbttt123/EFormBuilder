using System.ComponentModel.DataAnnotations;

namespace EFormBuilder.Application.DTOs.Form;

public class UpdateFormRequest
{
    [MaxLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
    public string? Title { get; set; }

    [MaxLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
    public string? Description { get; set; }

    public string? Status { get; set; }
}