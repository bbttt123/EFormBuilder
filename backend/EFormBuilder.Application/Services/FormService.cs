using EFormBuilder.Application.DTOs.Form;
using EFormBuilder.Application.Interfaces;
using EFormBuilder.Domain.Entities;
using EFormBuilder.Domain.Exceptions;
using EFormBuilder.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace EFormBuilder.Application.Services;

public class FormService : IFormService
{
    private readonly AppDbContext _context;

    public FormService(AppDbContext context)
    {
        _context = context;
    }

    // ─── CREATE ────────────────────────────────────────────────────────────────

    public async Task<FormDetailResponse> CreateAsync(Guid userId, CreateFormRequest request)
    {
        var slug = await GenerateUniqueSlugAsync(request.Slug ?? request.Title);

        var form = new Form
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Slug = slug,
            Status = "Draft",
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Forms.Add(form);
        await _context.SaveChangesAsync();

        return MapToDetail(form);
    }

    // ─── LIST ──────────────────────────────────────────────────────────────────

    public async Task<List<FormSummaryResponse>> GetAllByUserAsync(Guid userId)
    {
        var forms = await _context.Forms
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.UpdatedAt)
            .Select(f => new FormSummaryResponse
            {
                Id = f.Id,
                Title = f.Title,
                Description = f.Description,
                Slug = f.Slug,
                Status = f.Status,
                FieldCount = f.Fields.Count,
                ResponseCount = f.Responses.Count,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            })
            .ToListAsync();

        return forms;
    }

    // ─── DETAIL ────────────────────────────────────────────────────────────────

    public async Task<FormDetailResponse> GetBySlugAsync(Guid userId, string slug)
    {
        var form = await _context.Forms
            .Include(f => f.Fields.OrderBy(fd => fd.OrderIndex))
                .ThenInclude(fd => fd.FieldOptions.OrderBy(o => o.OrderIndex))
            .FirstOrDefaultAsync(f => f.Slug == slug && f.UserId == userId)
            ?? throw new BusinessException(ErrorCode.FormNotFound);

        return MapToDetail(form);
    }

    // ─── UPDATE ────────────────────────────────────────────────────────────────

    public async Task<FormDetailResponse> UpdateAsync(Guid userId, Guid formId, UpdateFormRequest request)
    {
        var form = await _context.Forms
            .Include(f => f.Fields.OrderBy(fd => fd.OrderIndex))
                .ThenInclude(fd => fd.FieldOptions.OrderBy(o => o.OrderIndex))
            .FirstOrDefaultAsync(f => f.Id == formId && f.UserId == userId)
            ?? throw new BusinessException(ErrorCode.FormNotFound);

        if (request.Title is not null)
            form.Title = request.Title;

        if (request.Description is not null)
            form.Description = request.Description;

        if (request.Status is not null)
        {
            var validStatuses = new[] { "Draft", "Published", "Closed" };
            if (!validStatuses.Contains(request.Status))
                throw new BusinessException(ErrorCode.InvalidFormStatus);

            form.Status = request.Status;
        }

        form.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToDetail(form);
    }

    // ─── DELETE ────────────────────────────────────────────────────────────────

    public async Task DeleteAsync(Guid userId, Guid formId)
    {
        var form = await _context.Forms
            .FirstOrDefaultAsync(f => f.Id == formId && f.UserId == userId)
            ?? throw new BusinessException(ErrorCode.FormNotFound);

        _context.Forms.Remove(form);
        await _context.SaveChangesAsync();
    }

    // ─── HELPERS ───────────────────────────────────────────────────────────────

    private async Task<string> GenerateUniqueSlugAsync(string source)
    {
        var baseSlug = Slugify(source);
        var slug = baseSlug;
        var counter = 1;

        while (await _context.Forms.AnyAsync(f => f.Slug == slug))
        {
            slug = $"{baseSlug}-{counter++}";
        }

        return slug;
    }

    private static string Slugify(string text)
    {
        // Lowercase, replace spaces with hyphens, strip special chars
        var slug = text.ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = slug.Trim('-');

        // Fallback nếu sau khi strip thành rỗng (vd title toàn tiếng Việt)
        if (string.IsNullOrEmpty(slug))
            slug = Guid.NewGuid().ToString("N")[..8];

        return slug.Length > 100 ? slug[..100] : slug;
    }

    private static FormDetailResponse MapToDetail(Form form) => new()
    {
        Id = form.Id,
        Title = form.Title,
        Description = form.Description,
        Slug = form.Slug,
        Status = form.Status,
        CreatedAt = form.CreatedAt,
        UpdatedAt = form.UpdatedAt,
        Fields = form.Fields.Select(f => new FieldResponse
        {
            Id = f.Id,
            Label = f.Label,
            FieldType = f.FieldType,
            Required = f.Required,
            OrderIndex = f.OrderIndex,
            Options = f.FieldOptions.Select(o => new FieldOptionResponse
            {
                Id = o.Id,
                Value = o.Value,
                OrderIndex = o.OrderIndex
            }).ToList()
        }).ToList()
    };
}