using System.ComponentModel.DataAnnotations;

namespace EFormBuilder.Application.DTOs.Form;

public class CreateFormRequest
{
    [Required(ErrorMessage = "Tiêu đề không được để trống")]
    [MaxLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
    public string Title { get; set; } = default!;

    [MaxLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
    public string? Description { get; set; }

    /// <summary>
    /// Slug dùng cho public URL. Nếu không truyền, server tự generate từ Title.
    /// </summary>
    [MaxLength(200)]
    public string? Slug { get; set; }
}