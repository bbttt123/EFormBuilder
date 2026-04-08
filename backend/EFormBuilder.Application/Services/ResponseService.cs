using EFormBuilder.Application.DTOs.FormResponse;
using EFormBuilder.Application.Interfaces;
using EFormBuilder.Domain.Exceptions;
using EFormBuilder.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EFormBuilder.Application.Services;

public class ResponseService : IResponseService
{
    private readonly AppDbContext _context;

    public ResponseService(AppDbContext context)
    {
        _context = context;
    }

    // ─── LIST ──────────────────────────────────────────────────────────────────

    public async Task<List<ResponseSummaryDto>> GetResponsesAsync(Guid userId, Guid formId)
    {
        // Verify form thuộc user
        var formExists = await _context.Forms
            .AnyAsync(f => f.Id == formId && f.UserId == userId);

        if (!formExists)
            throw new BusinessException(ErrorCode.FormNotFound);

        var responses = await _context.Responses
            .Where(r => r.FormId == formId)
            .OrderByDescending(r => r.SubmittedAt)
            .Select(r => new ResponseSummaryDto
            {
                Id = r.Id,
                ResponderEmail = r.ResponderEmail,
                SubmittedAt = r.SubmittedAt,
                AnsweredCount = r.Answers.Count(a => !string.IsNullOrEmpty(a.AnswerText))
            })
            .ToListAsync();

        return responses;
    }

    // ─── DETAIL ────────────────────────────────────────────────────────────────

    public async Task<ResponseDetailDto> GetResponseDetailAsync(Guid userId, Guid formId, Guid responseId)
    {
        // Verify form thuộc user
        var formExists = await _context.Forms
            .AnyAsync(f => f.Id == formId && f.UserId == userId);

        if (!formExists)
            throw new BusinessException(ErrorCode.FormNotFound);

        var response = await _context.Responses
            .Include(r => r.Answers)
                .ThenInclude(a => a.Field)
            .FirstOrDefaultAsync(r => r.Id == responseId && r.FormId == formId)
            ?? throw new BusinessException(ErrorCode.ResponseNotFound);

        return new ResponseDetailDto
        {
            Id = response.Id,
            ResponderEmail = response.ResponderEmail,
            SubmittedAt = response.SubmittedAt,
            Answers = response.Answers
                .OrderBy(a => a.Field.OrderIndex)
                .Select(a => new AnswerDetailDto
                {
                    FieldId = a.FieldId,
                    FieldLabel = a.Field.Label,
                    FieldType = a.Field.FieldType,
                    AnswerText = a.AnswerText
                })
                .ToList()
        };
    }

    // ─── ANALYTICS ─────────────────────────────────────────────────────────────

    public async Task<FormAnalyticsDto> GetAnalyticsAsync(Guid userId, Guid formId)
    {
        // Load form + fields (verify ownership)
        var form = await _context.Forms
            .Include(f => f.Fields.OrderBy(fd => fd.OrderIndex))
            .FirstOrDefaultAsync(f => f.Id == formId && f.UserId == userId)
            ?? throw new BusinessException(ErrorCode.FormNotFound);

        var totalResponses = await _context.Responses
            .CountAsync(r => r.FormId == formId);

        var lastSubmittedAt = await _context.Responses
            .Where(r => r.FormId == formId)
            .MaxAsync(r => (DateTime?)r.SubmittedAt);

        // Load tất cả answers của form này (dùng projection để không load toàn bộ entity)
        var allAnswers = await _context.Answers
            .Where(a => a.Response.FormId == formId && !string.IsNullOrEmpty(a.AnswerText))
            .Select(a => new { a.FieldId, a.AnswerText })
            .ToListAsync();

        // Group theo FieldId để tính aggregation
        var answersByField = allAnswers
            .GroupBy(a => a.FieldId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var fieldAnalytics = form.Fields.Select(field =>
        {
            var fieldAnswers = answersByField.TryGetValue(field.Id, out var fa) ? fa : new();

            // Aggregate: đếm số lần xuất hiện mỗi giá trị, lấy top 20
            var aggregations = fieldAnswers
                .GroupBy(a => a.AnswerText!)
                .OrderByDescending(g => g.Count())
                .Take(20)
                .Select(g => new AnswerAggregationDto
                {
                    Value = g.Key,
                    Count = g.Count()
                })
                .ToList();

            return new FieldAnalyticsDto
            {
                FieldId = field.Id,
                Label = field.Label,
                FieldType = field.FieldType,
                ResponseCount = fieldAnswers.Count,
                Aggregations = aggregations
            };
        }).ToList();

        return new FormAnalyticsDto
        {
            FormId = form.Id,
            FormTitle = form.Title,
            TotalResponses = totalResponses,
            LastSubmittedAt = lastSubmittedAt,
            Fields = fieldAnalytics
        };
    }
}