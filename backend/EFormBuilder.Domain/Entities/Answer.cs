namespace EFormBuilder.Domain.Entities;

public class Answer
{
    public Guid Id { get; set; }

    public string? AnswerText { get; set; }

    public Guid ResponseId { get; set; }

    public Response Response { get; set; } = default!;

    public Guid FieldId { get; set; }

    public Field Field { get; set; } = default!;
}