using System.Linq.Expressions;

namespace Hw11.Services.MathCalculator;

public class MathCalculatorService : IMathCalculatorService
{
    public async Task<double> CalculateMathExpressionAsync(string? expression)
    {
        ExpressionValidator.Validate(expression);
        var expr = ExpressionParser.CreateFromString(expression!);
        var compiled = Expression.Lambda<Func<double>>(
            await MyExpressionVisitor.VisitExpression((dynamic)expr));
        var result = compiled.Compile().Invoke();
        return result;
    }
}