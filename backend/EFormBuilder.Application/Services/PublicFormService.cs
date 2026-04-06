using EFormBuilder.Application.DTOs.PublicForm;
using EFormBuilder.Application.Interfaces;
using EFormBuilder.Domain.Entities;
using EFormBuilder.Domain.Exceptions;
using EFormBuilder.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EFormBuilder.Application.Services;

public class PublicFormService : IPublicFormService
{
    private readonly AppDbContext _context;

    public PublicFormService(AppDbContext context)
    {
        _context = context;
    }

    // ─── GET PUBLIC FORM ───────────────────────────────────────────────────────

    public async Task<PublicFormResponse> GetPublicFormAsync(string slug)
    {
        var form = await _context.Forms
            .Include(f => f.Fields.OrderBy(fd => fd.OrderIndex))
                .ThenInclude(fd => fd.FieldOptions.OrderBy(o => o.OrderIndex))
            .FirstOrDefaultAsync(f => f.Slug == slug)
            ?? throw new BusinessException(ErrorCode.FormNotFound);

        // Chỉ cho xem nếu form đang Published
        if (form.Status != "Published")
            throw new BusinessException(ErrorCode.FormNotPublished);

        return MapToPublicResponse(form);
    }

    // ─── SUBMIT ────────────────────────────────────────────────────────────────

    public async Task<SubmitFormResponse> SubmitAsync(string slug, SubmitFormRequest request)
    {
        // 1. Tìm form
        var form = await _context.Forms
            .Include(f => f.Fields)
            .FirstOrDefaultAsync(f => f.Slug == slug)
            ?? throw new BusinessException(ErrorCode.FormNotFound);

        if (form.Status != "Published")
            throw new BusinessException(ErrorCode.FormNotPublished);

        // 2. Kiểm tra email trùng (nếu có gửi email)
        if (!string.IsNullOrWhiteSpace(request.ResponderEmail))
        {
            var emailExists = await _context.Responses.AnyAsync(r =>
                r.FormId == form.Id &&
                r.ResponderEmail == request.ResponderEmail.Trim().ToLower());

            if (emailExists)
                throw new BusinessException(ErrorCode.DuplicateEmail);
        }

        // 3. Validate: tất cả FieldId trong request phải thuộc form này
        var formFieldIds = form.Fields.Select(f => f.Id).ToHashSet();
        var invalidFields = request.Answers
            .Select(a => a.FieldId)
            .Distinct()
            .Where(id => !formFieldIds.Contains(id))
            .ToList();

        if (invalidFields.Any())
            throw new BusinessException(ErrorCode.InvalidFieldReference);

        // 4. Validate: các field Required phải có AnswerText không rỗng
        var answersDict = request.Answers
            .GroupBy(a => a.FieldId)
            .ToDictionary(g => g.Key, g => g.First().AnswerText);

        var missingRequired = form.Fields
            .Where(f => f.Required)
            .Where(f => !answersDict.TryGetValue(f.Id, out var val) || string.IsNullOrWhiteSpace(val))
            .Select(f => f.Label)
            .ToList();

        if (missingRequired.Any())
            throw new BusinessException(
                ErrorCode.RequiredFieldMissing,
                $"Các trường bắt buộc chưa điền: {string.Join(", ", missingRequired)}");

        // 5. Tạo Response + Answers
        var response = new Response
        {
            Id = Guid.NewGuid(),
            FormId = form.Id,
            ResponderEmail = string.IsNullOrWhiteSpace(request.ResponderEmail)
                ? null
                : request.ResponderEmail.Trim().ToLower(),
            SubmittedAt = DateTime.UtcNow
        };

        // Chỉ tạo Answer cho những field thuộc form, bỏ qua field lạ đã được check trên
        response.Answers = request.Answers
            .Where(a => formFieldIds.Contains(a.FieldId))
            .Select(a => new Answer
            {
                Id = Guid.NewGuid(),
                ResponseId = response.Id,
                FieldId = a.FieldId,
                AnswerText = a.AnswerText?.Trim()
            })
            .ToList();

        _context.Responses.Add(response);
        await _context.SaveChangesAsync();

        return new SubmitFormResponse
        {
            ResponseId = response.Id,
            SubmittedAt = response.SubmittedAt
        };
    }

    // ─── HELPER ────────────────────────────────────────────────────────────────

    private static PublicFormResponse MapToPublicResponse(Form form) => new()
    {
        Id = form.Id,
        Title = form.Title,
        Description = form.Description,
        Slug = form.Slug,
        Fields = form.Fields.Select(f => new PublicFieldResponse
        {
            Id = f.Id,
            Label = f.Label,
            FieldType = f.FieldType,
            Required = f.Required,
            OrderIndex = f.OrderIndex,
            Options = f.FieldOptions.Select(o => new PublicFieldOptionResponse
            {
                Id = o.Id,
                Value = o.Value,
                OrderIndex = o.OrderIndex
            }).ToList()
        }).ToList()
    };
}