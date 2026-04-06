using EFormBuilder.Application.DTOs.PublicForm;

namespace EFormBuilder.Application.Interfaces;

public interface IPublicFormService
{
    Task<PublicFormResponse> GetPublicFormAsync(string slug);

    Task<SubmitFormResponse> SubmitAsync(string slug, SubmitFormRequest request);
}