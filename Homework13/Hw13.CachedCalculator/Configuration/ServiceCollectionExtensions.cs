using Hw13.CachedCalculator.Services;
using Hw13.CachedCalculator.Services.CachedCalculator;
using Hw13.CachedCalculator.Services.MathCalculator;
using Microsoft.Extensions.Caching.Memory;

namespace Hw13.CachedCalculator.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMathCalculator(this IServiceCollection services)
    {
        return services.AddTransient<MathCalculatorService>();
    }
    
    public static IServiceCollection AddCachedMathCalculator(this IServiceCollection services)
    {
        return services.AddScoped<IMathCalculatorService>(s =>
            new MathCachedCalculatorService(
                s.GetRequiredService<IMemoryCache>(), 
                s.GetRequiredService<MathCalculatorService>()));
    }
}