namespace EFormBuilder.Domain.Entities;

public class Field
{
    public Guid Id { get; set; }

    public string Label { get; set; } = string.Empty;

    public string FieldType { get; set; } = string.Empty;

    public bool Required { get; set; }

    public int OrderIndex { get; set; }

    public Guid FormId { get; set; }

    public Form Form { get; set; } = null!;

    public ICollection<FieldOption> FieldOptions { get; set; } = new List<FieldOption>();

    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}