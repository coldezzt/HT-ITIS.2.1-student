namespace Hw8.Calculator;

public class MyCalculator : ICalculator
{
    public double Plus(double val1, double val2) => val1 + val2;

    public double Minus(double val1, double val2) => val1 - val2;

    public double Multiply(double val1, double val2) => val1 * val2;

    public double Divide(double firstValue, double secondValue)
    {
        return Math.Round(secondValue, 10) == 0
            ? throw new DivideByZeroException(Messages.DivisionByZeroMessage) 
            : firstValue / secondValue;
    }
}