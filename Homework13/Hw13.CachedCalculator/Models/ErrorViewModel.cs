using System.Diagnostics.CodeAnalysis;

namespace Hw13.CachedCalculator.Models;

[ExcludeFromCodeCoverage]
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}