using System.Globalization;

namespace Hw8.Calculator;

public static class Parser
{
    public static (double value1, Operation operation, double value2) ParseArgs(string v1, string op, string v2)
    {
        if (!double.TryParse(v1, NumberStyles.Float, CultureInfo.InvariantCulture, out var val1) ||
            !double.TryParse(v2, NumberStyles.Float, CultureInfo.InvariantCulture, out var val2))
            throw new ArgumentException(Messages.InvalidNumberMessage);
        
        var operation = op.ToLower() switch
        {
            "plus" => Operation.Plus,
            "minus" => Operation.Minus,
            "multiply" => Operation.Multiply,
            "divide" => Operation.Divide,
            _ => Operation.Invalid
        };
        return (val1, operation, val2);
    }
}