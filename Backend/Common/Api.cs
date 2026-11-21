using System.Security.Claims;
namespace TeamLink.Backend.Common;
public sealed class ApiException(int status, string code, string message) : Exception(message)
{
    public int Status { get; } = status;
    public string Code { get; } = code;
}
public static class Api
{
    public static Guid UserId(this ClaimsPrincipal principal)
    {
        if (!Guid.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
            throw new ApiException(401, "UNAUTHORIZED", "Authentication required.");
        return id;
    }
    public static string Required(string? text, string field, int max = 4000)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Length > max)
            throw new ApiException(400, "INVALID_INPUT", $"{field} is required (max {max} characters).");
        return text.Trim();
    }
}
