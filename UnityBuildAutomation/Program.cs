using UnityBuildAutomation;

public class Program
{
    public static void Main(string[] args)
    {
        var applicationStartupDirector = new ApplicationStartupDirector();
        applicationStartupDirector.StartupTask().GetAwaiter().GetResult();
    }
}