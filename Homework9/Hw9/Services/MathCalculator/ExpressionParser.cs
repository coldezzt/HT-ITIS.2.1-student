using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;


namespace Hw9.Services.MathCalculator;

internal enum PartType
{
    Special,
    Number,
    Operator
}

internal class Part
{
    internal string Value { get; set; }
    internal PartType Type { get; set; }
}

public static class ExpressionParser
{
    private static Regex _numbers = new(@"^\d+");
    private static Regex _delimiters = new("(?<=[-+*/()])|(?=[-+*/()])");
    
    private static readonly Dictionary<string, int> OperatorPriorities = new()
    {
        { "(", 0 },
        { ")", 0 },
        { "+", 1 },
        { "-", 1 },
        { "*", 2 },
        { "/", 2 }
    };
    
    public static Expression CreateFromString(string s)
    {
        var reversePolish = ToReversePolishNotation(s);
        return CreateFromReversePolishNotation(reversePolish);
    }
    
    private static Expression CreateFromReversePolishNotation(List<Part> parts)
    {
        var buffer = new Stack<Expression>();
        foreach (var part in parts)
        {
            switch (part.Type)
            {
                case PartType.Number:
                {
                    buffer.Push(Expression.Constant(double.Parse(part.Value)));
                    break;
                }
                case PartType.Operator:
                {
                    Func<Expression, Expression, Expression> expr = part.Value switch
                    {
                        "/" => Expression.Divide,
                        "*" => Expression.Multiply,
                        "-" => Expression.Subtract,
                        "+" => Expression.Add,
                    };
                    Expression first = buffer.Pop(), second = buffer.Pop();
                    buffer.Push(expr(second, first)); // из-за использования стека
                    break;
                }
            }
        }

        return buffer.Pop();
    }
    
    internal static List<Part> ToReversePolishNotation(string infixExpression)
    {
        var ops = new Stack<string>();
        var result = new Stack<string>();
        var inputSplitted = _delimiters.Split(infixExpression.Replace(" ", ""));
        
        var isLastTokenOp = true;
        for (var i = 0; i < inputSplitted.Length; i++)
        {
            var token = inputSplitted[i];
            if (token.Length == 0) continue;
            if (_numbers.IsMatch(token))
            {
                result.Push(token);
                isLastTokenOp = false;
                continue;
            }
            if (token == "-" && isLastTokenOp)
            {
                result.Push(token + inputSplitted[++i]);
                isLastTokenOp = false;
                continue;
            }
            switch (token)
            {
                case "(":
                    ops.Push(token);
                    isLastTokenOp = true;
                    continue;
                case ")":
                {
                    while (ops.Peek() != "(")
                        PushOperation(ops, result);
                    ops.Pop();
                    isLastTokenOp = false;
                    continue;
                }
            }

            while (ops.Count > 0 && OperatorPriorities[token] <= OperatorPriorities[ops.Peek()])
                PushOperation(ops, result);
            
            ops.Push(token);
            isLastTokenOp = true;
        }

        while (ops.Count > 0)
            PushOperation(ops, result);

        return result.Pop()
            .Split(" ")
            .Select(x => new Part 
            {
                Value = x, 
                Type = double.TryParse(x, 
                    NumberStyles.Float, 
                    CultureInfo.InvariantCulture, 
                    out _) 
                    ? PartType.Number 
                    : x is "(" or ")" 
                        ? PartType.Special 
                        : PartType.Operator
            })
            .ToList();
    }
    
    private static void PushOperation(Stack<string> operations, Stack<string> polish)
    {
        var op = operations.Pop();
        var v1 = polish.Pop();
        var v2 = polish.Pop();
        polish.Push($"{v2} {v1} {op}");
    }
}