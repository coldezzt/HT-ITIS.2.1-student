using Hw13.CachedCalculator.Dto;

namespace Hw13.CachedCalculator.Services;

public interface IMathCalculatorService
{
    public Task<CalculationMathExpressionResultDto> CalculateMathExpressionAsync(string? expression);
}