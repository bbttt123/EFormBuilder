namespace EFormBuilder.Application.DTOs.Form;

public class FormSummaryResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string Slug { get; set; } = default!;
    public string Status { get; set; } = default!;
    public int FieldCount { get; set; }
    public int ResponseCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class FormDetailResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string Slug { get; set; } = default!;
    public string Status { get; set; } = default!;
    public List<FieldResponse> Fields { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class FieldResponse
{
    public Guid Id { get; set; }
    public string Label { get; set; } = default!;
    public string FieldType { get; set; } = default!;
    public bool Required { get; set; }
    public int OrderIndex { get; set; }
    public List<FieldOptionResponse> Options { get; set; } = new();
}

public class FieldOptionResponse
{
    public Guid Id { get; set; }
    public string Value { get; set; } = default!;
    public int OrderIndex { get; set; }
}