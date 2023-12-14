using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Hw13.CachedCalculator.ErrorMessages;

namespace Hw13.CachedCalculator.Services.MathCalculator;

[ExcludeFromCodeCoverage]
public static class ExpressionValidator
{
    public static void Validate(string? expression)
    {
        var message = "";
        var s = expression?.Replace(" ", "") ?? "";

        if (s is "")
            message = MathErrorMessager.EmptyString;
        
        else if (IndexOfUnknownCharacter(s) is not -1)
            message = MathErrorMessager.UnknownCharacterMessage(s[IndexOfUnknownCharacter(s)]);
        
        else if (StartsWithOperator(s))
            message = MathErrorMessager.StartingWithOperation;
        
        else if (EndsWithOperator(s))
            message = MathErrorMessager.EndingWithOperation;
        
        else if (ContainTwoOperatorsInRow(s, out var c1, out var c2))
            message = MathErrorMessager.TwoOperationInRowMessage(c1.ToString(), c2.ToString());
        
        else if (PartWithOperatorBeforeClosingBracket(s) is not "")
            message = MathErrorMessager.OperationBeforeParenthesisMessage(
                PartWithOperatorBeforeClosingBracket(s));
        
        else if (PartWithOperatorAfterOpeningBracket(s) is not "")
            message = MathErrorMessager.InvalidOperatorAfterParenthesisMessage(
                PartWithOperatorAfterOpeningBracket(s));
        
        else if (ContainIncorrectNumberOfBrackets(s))
            message = MathErrorMessager.IncorrectBracketsNumber;
        
        else if (ContainNotANumber(s) is not "")
            message = MathErrorMessager.NotNumberMessage(ContainNotANumber(s));

        if (message is not "")
            throw new ArgumentException(message);
    }

    private static string ContainNotANumber(string expression)
    {
        return expression
            .Split('+', '-', '/', '*', '(', ')')
            .Where(x => !string.IsNullOrEmpty(x))
            .FirstOrDefault(x => 
                !double.TryParse(x, NumberStyles.Float, CultureInfo.InvariantCulture, out _)) 
               ?? "";
    }

    private static bool ContainIncorrectNumberOfBrackets(string expression)
    {
        return expression.Select(x => x is '(' ? 1 : 0).Sum()
               != expression.Select(x => x is ')' ? 1 : 0).Sum();
    }
    
    private static string PartWithOperatorAfterOpeningBracket(string expression)
    {
        for(var i = 0; i < expression.Length - 1; i++)
            if (expression[i] is '(' && IsOperator(expression[i + 1]) && expression[i + 1] is not '-')
                return expression[i + 1].ToString();
        
        return "";
    }

    private static string PartWithOperatorBeforeClosingBracket(string expression)
    {
        for(var i = 0; i < expression.Length - 1; i++)
            if (IsOperator(expression[i]) && expression[i + 1] is ')')
                return expression[i].ToString();
        
        return "";
    }

    private static bool ContainTwoOperatorsInRow(string expression, out char f, out char s)
    {
        f = s = 'E';
        for (var i = 0; i < expression.Length - 1; i++)
            if (IsOperator(expression[i]) && IsOperator(expression[i + 1]))
            {
                f = expression[i];
                s = expression[i + 1];
                return true;
            }
        return false;
    }

    private static bool EndsWithOperator(string expression)=> IsOperator(expression[^1]);
    
    private static bool StartsWithOperator(string expression) => IsOperator(expression[0]);
    
    private static int IndexOfUnknownCharacter(string expression)
    {
        for (var i = 0; i < expression.Length; i++)
        {
            var c = expression[i];
            if (!char.IsDigit(c) 
                && !IsOperator(c) 
                && !new[] {'(', ')', '.', ' '}.Contains(c)) 
                return i;
        }

        return -1;
    }

    private static bool IsOperator(char c) => c is '+' or '-' or '/' or '*';
}