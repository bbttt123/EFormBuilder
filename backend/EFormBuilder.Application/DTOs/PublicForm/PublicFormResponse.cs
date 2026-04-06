namespace EFormBuilder.Application.DTOs.PublicForm;
public class PublicFormResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string Slug { get; set; } = default!;
    public List<PublicFieldResponse> Fields { get; set; } = new();
}

public class PublicFieldResponse
{
    public Guid Id { get; set; }
    public string Label { get; set; } = default!;
    public string FieldType { get; set; } = default!;
    public bool Required { get; set; }
    public int OrderIndex { get; set; }
    public List<PublicFieldOptionResponse> Options { get; set; } = new();
}

public class PublicFieldOptionResponse
{
    public Guid Id { get; set; }
    public string Value { get; set; } = default!;
    public int OrderIndex { get; set; }
}