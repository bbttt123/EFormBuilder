namespace EFormBuilder.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = default!;

    public string PasswordHash { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<Form> Forms { get; set; } = new List<Form>();
}