using System.Diagnostics.CodeAnalysis;
using Hw13.CachedCalculator.Dto;
using Hw13.CachedCalculator.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hw13.CachedCalculator.Controllers;

[ExcludeFromCodeCoverage]
public class CalculatorController : Controller
{
    private readonly IMathCalculatorService _mathCalculatorService;

    public CalculatorController(IMathCalculatorService mathCalculatorService)
    {
        _mathCalculatorService = mathCalculatorService;
    }

    [HttpGet]
    [ExcludeFromCodeCoverage]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult<CalculationMathExpressionResultDto>> CalculateMathExpression(string expression)
    {
        var result = await _mathCalculatorService.CalculateMathExpressionAsync(expression);
        return Json(result);
    }
}