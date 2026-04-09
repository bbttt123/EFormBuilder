using EFormBuilder.Application.DTOs.Field;
using EFormBuilder.Application.Interfaces;
using EFormBuilder.Domain.Entities;
using EFormBuilder.Domain.Exceptions;
using EFormBuilder.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EFormBuilder.Application.Services;

public class FieldService : IFieldService
{
    private readonly AppDbContext _context;

    public FieldService(AppDbContext context)
    {
        _context = context;
    }

    // ─── ADD FIELD ─────────────────────────────────────────────

    public async Task AddFieldAsync(Guid userId, Guid formId, CreateFieldRequest request)
    {
        var form = await _context.Forms
            .Include(f => f.Fields)
            .FirstOrDefaultAsync(f => f.Id == formId && f.UserId == userId)
            ?? throw new BusinessException(ErrorCode.FormNotFound);

        var orderIndex = form.Fields.Count + 1;

        var field = new Field
        {
            Id = Guid.NewGuid(),
            Label = request.Label,
            FieldType = request.FieldType,
            Required = request.Required,
            OrderIndex = orderIndex,
            FormId = formId
        };

        _context.Fields.Add(field);
        await _context.SaveChangesAsync();
    }

    // ─── UPDATE FIELD ─────────────────────────────────────────

    public async Task UpdateFieldAsync(Guid userId, Guid fieldId, UpdateFieldRequest request)
    {
        var field = await _context.Fields
            .Include(f => f.Form)
            .FirstOrDefaultAsync(f => f.Id == fieldId && f.Form.UserId == userId)
            ?? throw new BusinessException(ErrorCode.FieldNotFound);

        if (request.Label is not null)
            field.Label = request.Label;

        if (request.FieldType is not null)
            field.FieldType = request.FieldType;

        if (request.Required.HasValue)
            field.Required = request.Required.Value;

        await _context.SaveChangesAsync();
    }

    // ─── DELETE FIELD ─────────────────────────────────────────

    public async Task DeleteFieldAsync(Guid userId, Guid fieldId)
    {
        var field = await _context.Fields
            .Include(f => f.Form)
            .FirstOrDefaultAsync(f => f.Id == fieldId && f.Form.UserId == userId)
            ?? throw new BusinessException(ErrorCode.FieldNotFound);

        _context.Fields.Remove(field);
        await _context.SaveChangesAsync();
    }

    // ─── REORDER ─────────────────────────────────────────────

    public async Task ReorderFieldsAsync(Guid userId, Guid formId, List<ReorderFieldRequest> request)
    {
        var form = await _context.Forms
            .Include(f => f.Fields)
            .FirstOrDefaultAsync(f => f.Id == formId && f.UserId == userId)
            ?? throw new BusinessException(ErrorCode.FormNotFound);

        foreach (var item in request)
        {
            var field = form.Fields.FirstOrDefault(f => f.Id == item.Id);
            if (field != null)
            {
                field.OrderIndex = item.OrderIndex;
            }
        }

        await _context.SaveChangesAsync();
    }

    // ─── ADD FIELD OPTIONS ────────────────────────────────────────

    public async Task AddFieldOptionAsync(Guid userId, Guid fieldId, string value)
    {
        // Kiểm tra field có tồn tại và thuộc về user không
        var field = await _context.Fields
            .Include(f => f.Form)
            .Include(f => f.FieldOptions)
            .FirstOrDefaultAsync(f => f.Id == fieldId && f.Form.UserId == userId)
            ?? throw new BusinessException(ErrorCode.FieldNotFound);

        var option = new FieldOption
        {
            Id = Guid.NewGuid(),
            FieldId = fieldId,
            Value = value,
            OrderIndex = field.FieldOptions.Count
        };

        _context.FieldOptions.Add(option);
        await _context.SaveChangesAsync();
    }

    // ─── UPDATE FIELD OPTIONS ────────────────────────────────────────
    public async Task UpdateFieldOptionAsync(Guid userId, Guid optionId, string value)
    {
        var option = await _context.FieldOptions
            .Include(o => o.Field).ThenInclude(f => f.Form)
            .FirstOrDefaultAsync(o => o.Id == optionId && o.Field.Form.UserId == userId)
            ?? throw new BusinessException(ErrorCode.FieldNotFound);

        option.Value = value;
        await _context.SaveChangesAsync();
    }

    // ─── DELETE FIELD OPTIONS ────────────────────────────────────────
    public async Task DeleteFieldOptionAsync(Guid userId, Guid optionId)
    {
        var option = await _context.FieldOptions
            .Include(o => o.Field).ThenInclude(f => f.Form)
            .FirstOrDefaultAsync(o => o.Id == optionId && o.Field.Form.UserId == userId)
            ?? throw new BusinessException(ErrorCode.FieldNotFound);

        _context.FieldOptions.Remove(option);
        await _context.SaveChangesAsync();
    }
}