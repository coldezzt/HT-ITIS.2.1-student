using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Tests.CSharp.Homework3;

public class SingleInitializationSingleton
{
    public const int DefaultDelay = 3_000;

    private static volatile bool _isInitialized = false;
    private static Lazy<SingleInitializationSingleton> _instance = new();
    
    private SingleInitializationSingleton(int delay = DefaultDelay)
    {
        Delay = delay;
        // imitation of complex initialization logic
        Thread.Sleep(delay);
    }

    public int Delay { get; }

    public static SingleInitializationSingleton Instance => _instance.Value;

    internal static void Reset()
    {
        _instance = new Lazy<SingleInitializationSingleton>(() => new SingleInitializationSingleton());
        _isInitialized = false;
    }

    public static void Initialize(int delay)
    {
        if (_isInitialized) 
            throw new InvalidOperationException();
        _instance = new Lazy<SingleInitializationSingleton>(() => new SingleInitializationSingleton(delay));
        _isInitialized = true;
    }
}