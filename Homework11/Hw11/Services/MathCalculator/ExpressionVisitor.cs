using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Hw11.ErrorMessages;

namespace Hw11.Services.MathCalculator;

[ExcludeFromCodeCoverage]
public static class MyExpressionVisitor
{
    public static Task<Expression> VisitExpression(Expression expr) => 
        CreateExpression((dynamic) expr);

    private static async Task<Expression> CreateExpression(ConstantExpression expr) => 
        Expression.Constant((double) expr.Value!);

    private static async Task<Expression> CreateExpression(BinaryExpression expression)
    {
        var values = new List<double>(await CompileNodes(expression.Left, expression.Right));

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
    
    private static async Task<double[]> CompileNodes(params Expression[] nodes)
    {
        await Task.Delay(1000); // имитация работы
        var tasks = new List<Task<double>>();
        foreach (var node in nodes)
        {
                tasks.Add(Task.Run(() => Expression
                    .Lambda<Func<double>>(VisitExpression(node).Result)
                    .Compile()
                    .Invoke()));
        }
        await Task.WhenAll(tasks);
        return tasks.Select(x => x.Result).ToArray();
    }
}