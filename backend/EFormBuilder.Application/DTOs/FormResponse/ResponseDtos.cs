namespace EFormBuilder.Application.DTOs.FormResponse;
public class ResponseSummaryDto
{
    public Guid Id { get; set; }
    public string? ResponderEmail { get; set; }
    public DateTime SubmittedAt { get; set; }

    public int AnsweredCount { get; set; }
}


public class ResponseDetailDto
{
    public Guid Id { get; set; }
    public string? ResponderEmail { get; set; }
    public DateTime SubmittedAt { get; set; }
    public List<AnswerDetailDto> Answers { get; set; } = new();
}

public class AnswerDetailDto
{
    public Guid FieldId { get; set; }
    public string FieldLabel { get; set; } = default!;
    public string FieldType { get; set; } = default!;
    public string? AnswerText { get; set; }
}


public class FormAnalyticsDto
{
    public Guid FormId { get; set; }
    public string FormTitle { get; set; } = default!;
    public int TotalResponses { get; set; }
    public DateTime? LastSubmittedAt { get; set; }
    public List<FieldAnalyticsDto> Fields { get; set; } = new();
}

public class FieldAnalyticsDto
{
    public Guid FieldId { get; set; }
    public string Label { get; set; } = default!;
    public string FieldType { get; set; } = default!;

    public int ResponseCount { get; set; }

    public List<AnswerAggregationDto> Aggregations { get; set; } = new();
}

public class AnswerAggregationDto
{
    public string Value { get; set; } = default!;
    public int Count { get; set; }
}