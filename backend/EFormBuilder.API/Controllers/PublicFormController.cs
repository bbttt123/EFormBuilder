using EFormBuilder.Application.DTOs;
using EFormBuilder.Application.DTOs.PublicForm;
using EFormBuilder.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EFormBuilder.API.Controllers;

/// 
/// Các endpoint public — KHÔNG cần JWT.
/// Route: /api/public/forms
/// 
[ApiController]
[Route("api/public/forms")]
public class PublicFormController : ControllerBase
{
    private readonly IPublicFormService _publicFormService;

    public PublicFormController(IPublicFormService publicFormService)
    {
        _publicFormService = publicFormService;
    }

    /// 
    /// GET /api/public/forms/{slug}
    /// Xem form trước khi submit — chỉ trả về nếu Status = "Published"
    /// 
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetForm(string slug)
    {
        var result = await _publicFormService.GetPublicFormAsync(slug);
        return Ok(ApiResponse<PublicFormResponse>.SuccessResponse(result));
    }

    /// 
    /// POST /api/public/forms/{slug}/submit
    /// Nộp câu trả lời cho form.
    /// - Tạo Response + Answers
    /// - Validate Required fields
    /// - Check unique email (nếu có email)
    /// 
    [HttpPost("{slug}/submit")]
    public async Task<IActionResult> Submit(string slug, [FromBody] SubmitFormRequest request)
    {
        var result = await _publicFormService.SubmitAsync(slug, request);
        return StatusCode(201, ApiResponse<SubmitFormResponse>.SuccessResponse(result));
    }
}