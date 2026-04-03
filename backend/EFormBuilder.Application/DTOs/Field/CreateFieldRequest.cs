namespace EFormBuilder.Application.DTOs.Field;

public class CreateFieldRequest
{
    public string Label { get; set; }
    public string FieldType { get; set; } // Text, Number, Select...
    public bool Required { get; set; }
}