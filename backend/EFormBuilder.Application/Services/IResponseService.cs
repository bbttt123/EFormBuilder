using EFormBuilder.Application.DTOs.FormResponse;

namespace EFormBuilder.Application.Interfaces;

public interface IResponseService
{
    Task<List<ResponseSummaryDto>> GetResponsesAsync(Guid userId, Guid formId);

    Task<ResponseDetailDto> GetResponseDetailAsync(Guid userId, Guid formId, Guid responseId);

    Task<FormAnalyticsDto> GetAnalyticsAsync(Guid userId, Guid formId);
}