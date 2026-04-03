namespace EFormBuilder.Domain.Exceptions;

public class AppValidationException : Exception
{
    public List<EFormBuilder.Application.DTOs.ValidationError> Errors { get; }

    public AppValidationException(List<EFormBuilder.Application.DTOs.ValidationError> errors)
        : base("Validation failed")
    {
        Errors = errors;
    }
}