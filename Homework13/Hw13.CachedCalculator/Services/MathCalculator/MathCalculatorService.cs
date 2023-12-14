using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Hw13.CachedCalculator.Dto;

namespace Hw13.CachedCalculator.Services.MathCalculator;

[ExcludeFromCodeCoverage]
public class MathCalculatorService : IMathCalculatorService
{
    public async Task<CalculationMathExpressionResultDto> CalculateMathExpressionAsync(string? expression)
    {
        try
        {
            ExpressionValidator.Validate(expression);
            var expr = ExpressionParser.CreateFromString(expression!);
            var compiled = Expression.Lambda<Func<double>>(
                await BinaryExpressionVisitor.VisitExpression(expr));
            var result = compiled.Compile().Invoke();
            return new CalculationMathExpressionResultDto(result);
        }
        catch (Exception ex)
        {
            return new CalculationMathExpressionResultDto(ex.Message);
        }
    }
}