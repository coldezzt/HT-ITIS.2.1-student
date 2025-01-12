using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Tests.CSharp.Homework3;

public class SingleInitializationSingleton
{
    public const int DefaultDelay = 3_000;

    private static object Locker = new object();
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
        lock (Locker)
        {
            _instance = new Lazy<SingleInitializationSingleton>(
                () => new SingleInitializationSingleton()
            );
            _isInitialized = false;    
        }
    }

    public static void Initialize(int delay)
    {
        if (_isInitialized) 
            throw new InvalidOperationException("Object already initialized once");

        lock (Locker)
        {
            if (_isInitialized)
                throw new InvalidOperationException("Object already initialized once");
            
            _instance = new Lazy<SingleInitializationSingleton>(
                () => new SingleInitializationSingleton(delay)
            );
            _isInitialized = true;
        }
    }
}