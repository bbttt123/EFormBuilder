namespace EFormBuilder.Application.DTOs; 

public class ValidationError
{
    public string? Field { get; set; }
    public string? Message { get; set; }
    public string? Annotation { get; set; }
    public Dictionary<string, object>? Params { get; set; }
}