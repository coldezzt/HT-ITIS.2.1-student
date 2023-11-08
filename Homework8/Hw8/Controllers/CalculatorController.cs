using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Hw8.Calculator;
using Microsoft.AspNetCore.Mvc;

namespace Hw8.Controllers;

public class CalculatorController : Controller
{
    public ActionResult<double> Calculate([FromServices] ICalculator calculator,
        string val1,
        string operation,
        string val2)
    {
        try
        {
            var (x, op, y) = Parser.ParseArgs(val1, operation, val2);
            return op switch
            {
                Operation.Plus => calculator.Plus(x, y),
                Operation.Minus => calculator.Minus(x, y),
                Operation.Multiply => calculator.Multiply(x, y),
                Operation.Divide => calculator.Divide(x, y),
                Operation.Invalid => throw new ArgumentException(Messages.InvalidOperationMessage)
            };
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [ExcludeFromCodeCoverage]
    public IActionResult Index()
    {
        return Content(
            "Заполните val1, operation(plus, minus, multiply, divide) и val2 здесь '/calculator/calculate?val1= &operation= &val2= '\n" +
            "и добавьте её в адресную строку.");
    }
}