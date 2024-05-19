using UnityBuildAutomation;

public class Program
{
    public static void Main(string[] args)
    {
        var applicationStartupDirector = new ApplicationStartupDirector();
        applicationStartupDirector.StartupTask().GetAwaiter().GetResult();
        Console.WriteLine("Startup complete.");
        var executionEngine = new ExecutionEngine();
        executionEngine.EnterOptions();
    }
}