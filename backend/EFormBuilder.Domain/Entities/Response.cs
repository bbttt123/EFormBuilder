namespace EFormBuilder.Domain.Entities;

public class Response
{
	public Guid Id { get; set; }

	public string? ResponderEmail { get; set; }

	public DateTime SubmittedAt { get; set; }

	public Guid FormId { get; set; }

	public Form Form { get; set; } = default!;

	public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}