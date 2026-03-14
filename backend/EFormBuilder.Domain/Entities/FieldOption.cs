namespace EFormBuilder.Domain.Entities;

public class FieldOption
{
	public Guid Id { get; set; }

	public string Value { get; set; } = default!;

	public int OrderIndex { get; set; }

	public Guid FieldId { get; set; }

	public Field Field { get; set; } = default!;
}