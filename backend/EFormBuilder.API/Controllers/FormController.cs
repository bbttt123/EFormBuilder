using EFormBuilder.Application.DTOs;
using EFormBuilder.Application.DTOs.Form;
using EFormBuilder.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EFormBuilder.API.Controllers;

[ApiController]
[Route("api/forms")]
[Authorize]
public class FormController : ControllerBase
{
    private readonly IFormService _formService;

    public FormController(IFormService formService)
    {
        _formService = formService;
    }

    /// POST /api/forms — Tạo form mới
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFormRequest request)
    {
        var userId = GetUserId();
        var result = await _formService.CreateAsync(userId, request);
        return StatusCode(201, ApiResponse<FormDetailResponse>.SuccessResponse(result));
    }

    /// GET /api/forms — Danh sách form của creator
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        var result = await _formService.GetAllByUserAsync(userId);
        return Ok(ApiResponse<List<FormSummaryResponse>>.SuccessResponse(result));
    }

    /// GET /api/forms/{id} — Chi tiết form + fields
    [Authorize]
    [HttpGet("owner/slug/{slug}")]
    public async Task<IActionResult> GetBySlugForOwner(string slug)
    {
        var userId = GetUserId();

        var result = await _formService.GetBySlugAsync(userId, slug);

        return Ok(ApiResponse<FormDetailResponse>.SuccessResponse(result));
    }

    /// PUT /api/forms/{id} — Cập nhật title / description
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFormRequest request)
    {
        var userId = GetUserId();
        var result = await _formService.UpdateAsync(userId, id, request);
        return Ok(ApiResponse<FormDetailResponse>.SuccessResponse(result));
    }

    /// DELETE /api/forms/{id} — Xóa form
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        await _formService.DeleteAsync(userId, id);
        return Ok(ApiResponse<string>.SuccessResponse("Xóa form thành công"));
    }

    /// PUT /api/forms/{id}/status — Đổi trạng thái form
    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> SwitchStatus(Guid id, [FromBody] UpdateFormStatusRequest request)
    {
        var userId = GetUserId();

        await _formService.SwitchStatusAsync(request.Status, id, userId);

        return Ok(ApiResponse<string>.SuccessResponse("Cập nhật trạng thái thành công"));
    }

    // ─── HELPER ───────────────────────────────────────────────────────────────

    private Guid GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Không xác định được người dùng");
        return Guid.Parse(claim);
    }
}