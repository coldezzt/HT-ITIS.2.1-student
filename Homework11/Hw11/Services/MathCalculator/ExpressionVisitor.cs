using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Hw11.ErrorMessages;

namespace Hw11.Services.MathCalculator;

[ExcludeFromCodeCoverage]
public static class MyExpressionVisitor
{
    public static Task<Expression> VisitExpression(Expression expr) =>
        Task.Run(() => VisitDispatcher(expr));

    private static Expression VisitDispatcher(Expression expr)
    {
        Thread.Sleep(800); // имитация работы
        try
        {
            return Visit((dynamic) expr).Result;
        }
        catch
        {
            throw new Exception(MathErrorMessager.DivisionByZero);
        }
    }
    
    private static async Task<Expression> Visit(ConstantExpression node)
    {
        return CreateExpression(node, (double) node.Value! );
    }
    
    private static async Task<Expression> Visit(BinaryExpression node)
    {
        var expressionValues = new List<double>(await CompileNodes(node.Left, node.Right));
        return CreateExpression(node, expressionValues);
    }
    
    private static async Task<double[]> CompileNodes(params Expression[] nodes)
    {
        var tasks = nodes
            .Select(node => 
                Task.Run(() => Expression
                    .Lambda<Func<double>>(VisitExpression(node).Result)
                    .Compile()
                    .Invoke()))
            .ToList();
        await Task.WhenAll(tasks);
        return tasks.Select(x => x.Result).ToArray();
    }

    private static Expression CreateExpression(ConstantExpression expr, double value)
    {
        return Expression.Constant(value);
    }
    
    private static Expression CreateExpression(BinaryExpression expression, IReadOnlyList<double> values)
    {
        if (expression.NodeType is ExpressionType.Divide 
            && Math.Abs(values[1] - double.Epsilon) < 0.00001)
            throw new Exception(MathErrorMessager.DivisionByZero);
        
        Func<Expression, Expression, Expression> expr = expression.NodeType switch
        {
            ExpressionType.Add => Expression.Add,
            ExpressionType.Subtract => Expression.Subtract,
            ExpressionType.Multiply => Expression.Multiply,
            ExpressionType.Divide => Expression.Divide
        };

        return expr(Expression.Constant(values[0]), Expression.Constant(values[1]));
    }
}