namespace EFormBuilder.Application.DTOs.Field;

public class UpdateFieldRequest
{
    public string? Label { get; set; }
    public string? FieldType { get; set; }
    public bool? Required { get; set; }
}