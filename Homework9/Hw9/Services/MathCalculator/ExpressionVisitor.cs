using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Hw9.ErrorMessages;

namespace Hw9.Services.MathCalculator;

[ExcludeFromCodeCoverage]
public class BinaryExpressionVisitor : ExpressionVisitor
{
    public static Task<Expression> VisitExpression(Expression expression) =>
        Task.Run(() => new BinaryExpressionVisitor().Visit(expression));
    
    protected override Expression VisitBinary(BinaryExpression node)
    {
        var expressionValues = CompileAsync(node.Left, node.Right).Result;
        return GetExpressionByType(node.NodeType, expressionValues);
    }
    
    private static async Task<double[]> CompileAsync(Expression left, Expression right)
    {
        await Task.Delay(1000); // имитация работы
        var t1 = Task.Run(() => Expression.Lambda<Func<double>>(left).Compile().Invoke());
        var t2 = Task.Run(() => Expression.Lambda<Func<double>>(right).Compile().Invoke());
        return await Task.WhenAll(t1, t2);
    }
    
    private static Expression GetExpressionByType(ExpressionType expressionType, IReadOnlyList<double> values)
    {
        Func<Expression, Expression, Expression> expr = expressionType switch
        {
            ExpressionType.Add => Expression.Add,
            ExpressionType.Subtract => Expression.Subtract,
            ExpressionType.Multiply => Expression.Multiply,
            ExpressionType.Divide => Expression.Divide
        };
        if (expressionType is ExpressionType.Divide && values[1] <= double.Epsilon)
            throw new DivideByZeroException(MathErrorMessager.DivisionByZero);

        return expr(Expression.Constant(values[0]), Expression.Constant(values[1]));
    }
}