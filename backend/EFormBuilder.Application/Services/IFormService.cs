using EFormBuilder.Application.DTOs.Form;

namespace EFormBuilder.Application.Interfaces;

public interface IFormService
{
    Task<FormDetailResponse> CreateAsync(Guid userId, CreateFormRequest request);
    Task<List<FormSummaryResponse>> GetAllByUserAsync(Guid userId);
    Task<FormDetailResponse> GetBySlugAsync(Guid userId, string slug);
    Task<FormDetailResponse> UpdateAsync(Guid userId, Guid formId, UpdateFormRequest request);
    Task DeleteAsync(Guid userId, Guid formId);
}