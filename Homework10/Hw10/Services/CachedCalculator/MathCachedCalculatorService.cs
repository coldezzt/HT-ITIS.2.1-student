using Hw10.DbModels;
using Hw10.Dto;
using Hw10.Services.MathCalculator;
using Microsoft.EntityFrameworkCore;

namespace Hw10.Services.CachedCalculator;

public class MathCachedCalculatorService : IMathCalculatorService
{
	private readonly ApplicationContext _dbContext;
	private readonly IMathCalculatorService _simpleCalculator;

	public MathCachedCalculatorService(ApplicationContext dbContext, IMathCalculatorService simpleCalculator)
	{
		_dbContext = dbContext;
		_simpleCalculator = simpleCalculator;
	}

	public async Task<CalculationMathExpressionResultDto> CalculateMathExpressionAsync(string? expression)
	{
		var dbSet = _dbContext.Set<SolvingExpression>();
		if (await dbSet.AnyAsync(x => x.Expression.Equals(expression)))
		{
			var foundedExpression = await dbSet.FirstAsync(x => x.Expression.Equals(expression));
			return new CalculationMathExpressionResultDto(foundedExpression.Result);
		}

		var result = await _simpleCalculator.CalculateMathExpressionAsync(expression);
		if (!result.IsSuccess) return result;
		
		await dbSet.AddAsync(new SolvingExpression {Expression = expression, Result = result.Result});
		await _dbContext.SaveChangesAsync();
		return result;
	}
}