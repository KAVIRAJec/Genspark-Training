using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DotnetCoreMigration.Extensions;

public static class ModelStateExtensions
{
    public static Dictionary<string, string[]> GetValidationErrors(this ModelStateDictionary modelState)
    {
        return modelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? new string[0]
            );
    }

    public static string GetErrorMessage(this ModelStateDictionary modelState)
    {
        var errors = modelState.GetValidationErrors();
        var errorMessages = errors.SelectMany(kvp => kvp.Value.Select(error => $"{kvp.Key}: {error}"));
        return string.Join("; ", errorMessages);
    }
}
