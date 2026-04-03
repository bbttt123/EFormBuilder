using EFormBuilder.Application.DTOs;
using EFormBuilder.Application.DTOs.Field;
using EFormBuilder.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EFormBuilder.API.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class FieldController : ControllerBase
{
    private readonly IFieldService _fieldService;

    public FieldController(IFieldService fieldService)
    {
        _fieldService = fieldService;
    }

    // ─── ADD FIELD ─────────────────────────────────────────

    /// <summary>POST /api/forms/{formId}/fields — Thêm field vào form</summary>
    [HttpPost("forms/{formId:guid}/fields")]
    public async Task<IActionResult> AddField(Guid formId, [FromBody] CreateFieldRequest request)
    {
        var userId = GetUserId();
        await _fieldService.AddFieldAsync(userId, formId, request);

        return StatusCode(201, ApiResponse<string>.SuccessResponse("Thêm field thành công"));
    }

    // ─── UPDATE FIELD ──────────────────────────────────────

    /// <summary>PUT /api/fields/{fieldId} — Cập nhật field</summary>
    [HttpPut("fields/{fieldId:guid}")]
    public async Task<IActionResult> UpdateField(Guid fieldId, [FromBody] UpdateFieldRequest request)
    {
        var userId = GetUserId();
        await _fieldService.UpdateFieldAsync(userId, fieldId, request);

        return Ok(ApiResponse<string>.SuccessResponse("Cập nhật field thành công"));
    }

    // ─── DELETE FIELD ──────────────────────────────────────

    /// <summary>DELETE /api/fields/{fieldId} — Xóa field</summary>
    [HttpDelete("fields/{fieldId:guid}")]
    public async Task<IActionResult> DeleteField(Guid fieldId)
    {
        var userId = GetUserId();
        await _fieldService.DeleteFieldAsync(userId, fieldId);

        return Ok(ApiResponse<string>.SuccessResponse("Xóa field thành công"));
    }

    // ─── REORDER FIELDS ────────────────────────────────────

    /// <summary>PUT /api/forms/{formId}/fields/reorder — Sắp xếp lại field</summary>
    [HttpPut("forms/{formId:guid}/fields/reorder")]
    public async Task<IActionResult> ReorderFields(Guid formId, [FromBody] List<ReorderFieldRequest> request)
    {
        var userId = GetUserId();
        await _fieldService.ReorderFieldsAsync(userId, formId, request);

        return Ok(ApiResponse<string>.SuccessResponse("Sắp xếp field thành công"));
    }

    // ─── HELPER ────────────────────────────────────────────

    private Guid GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Không xác định được người dùng");

        return Guid.Parse(claim);
    }
}