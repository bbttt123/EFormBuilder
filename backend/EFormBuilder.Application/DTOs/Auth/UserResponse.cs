namespace EFormBuilder.Application.DTOs.Auth;

public class UserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

}