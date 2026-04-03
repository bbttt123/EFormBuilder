using EFormBuilder.Application.DTOs.Field;

public interface IFieldService
{
    Task AddFieldAsync(Guid userId, Guid formId, CreateFieldRequest request);
    Task UpdateFieldAsync(Guid userId, Guid fieldId, UpdateFieldRequest request);
    Task DeleteFieldAsync(Guid userId, Guid fieldId);
    Task ReorderFieldsAsync(Guid userId, Guid formId, List<ReorderFieldRequest> request);
}