using EFormBuilder.Application.DTOs;
using EFormBuilder.Application.DTOs.FormResponse;
using EFormBuilder.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EFormBuilder.API.Controllers;

/// 
/// Creator xem responses của form mình tạo — cần JWT.
/// Route: /api/forms/{id}/responses  và  /api/forms/{id}/analytics
/// 
[ApiController]
[Route("api/forms/{formId:guid}")]
[Authorize]
public class ResponseController : ControllerBase
{
    private readonly IResponseService _responseService;

    public ResponseController(IResponseService responseService)
    {
        _responseService = responseService;
    }

    /// 
    /// GET /api/forms/{formId}/responses
    /// Danh sách tất cả responses (tóm tắt) của form — chỉ owner mới xem được
    /// 
    [HttpGet("responses")]
    public async Task<IActionResult> GetResponses(Guid formId)
    {
        var userId = GetUserId();
        var result = await _responseService.GetResponsesAsync(userId, formId);
        return Ok(ApiResponse<List<ResponseSummaryDto>>.SuccessResponse(result));
    }

    /// 
    /// GET /api/forms/{formId}/responses/{responseId}
    /// Chi tiết 1 response — gồm đầy đủ từng câu trả lời kèm label field
    /// 
    [HttpGet("responses/{responseId:guid}")]
    public async Task<IActionResult> GetResponseDetail(Guid formId, Guid responseId)
    {
        var userId = GetUserId();
        var result = await _responseService.GetResponseDetailAsync(userId, formId, responseId);
        return Ok(ApiResponse<ResponseDetailDto>.SuccessResponse(result));
    }

    /// 
    /// GET /api/forms/{formId}/analytics
    /// Tổng hợp thống kê: tổng response, từng field có bao nhiêu câu trả lời,
    /// top giá trị thường gặp (dùng cho radio/checkbox/text)
    /// 
    [HttpGet("analytics")]
    public async Task<IActionResult> GetAnalytics(Guid formId)
    {
        var userId = GetUserId();
        var result = await _responseService.GetAnalyticsAsync(userId, formId);
        return Ok(ApiResponse<FormAnalyticsDto>.SuccessResponse(result));
    }

    // ─── HELPER ───────────────────────────────────────────────────────────────

    private Guid GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Không xác định được người dùng");
        return Guid.Parse(claim);
    }
}