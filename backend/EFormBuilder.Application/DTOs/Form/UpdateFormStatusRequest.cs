using System.ComponentModel.DataAnnotations;

namespace EFormBuilder.Application.DTOs.Form;

public class UpdateFormStatusRequest
{
    public string? Status { get; set; } = default!;
}