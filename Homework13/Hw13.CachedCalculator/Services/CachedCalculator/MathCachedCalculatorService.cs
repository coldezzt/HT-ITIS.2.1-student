using Hw13.CachedCalculator.Dto;
using Microsoft.Extensions.Caching.Memory;

namespace Hw13.CachedCalculator.Services.CachedCalculator;

public class MathCachedCalculatorService : IMathCalculatorService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IMathCalculatorService _simpleCalculator;

    public MathCachedCalculatorService(IMemoryCache memoryCache, IMathCalculatorService simpleCalculator)
    {
        _simpleCalculator = simpleCalculator;
        _memoryCache = memoryCache;
    }

    public async Task<CalculationMathExpressionResultDto> CalculateMathExpressionAsync(string? expression)
    {
        return await _memoryCache.GetOrCreateAsync(expression, async _ =>
        {
            var res = await _simpleCalculator.CalculateMathExpressionAsync(expression);
            return res.IsSuccess
                ? new CalculationMathExpressionResultDto(res.Result)
                : new CalculationMathExpressionResultDto(res.ErrorMessage);
        });
    }
}