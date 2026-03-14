namespace EFormBuilder.Domain.Entities;

public class Form
{
    public Guid Id { get; set; }

    public string Title { get; set; } = default!;

    public string? Description { get; set; }

    public string Status { get; set; } = "Draft";

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = default!;

    public ICollection<Field> Fields { get; set; } = new List<Field>();

    public ICollection<Response> Responses { get; set; } = new List<Response>();
}