using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityBuildAutomation
{
    internal class ExecutionEngine
    {
        private readonly Configuration configuration;

        internal ExecutionEngine()
        {
            this.configuration = ConfigurationManager.LoadConfiguration();
        }

        const string Options = """
            OPTIONS:
            1. Build Latest
            """;

        public void EnterOptions()
        {
            Console.WriteLine(Options);
            var option = Console.ReadLine() ?? string.Empty;
            switch (option)
            {
                case "1":
                    BuildLatest().GetAwaiter().GetResult();
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }

        async Task BuildLatest()
        {
            Console.WriteLine("Building latest...");
            var sourceControl = new SourceControl.ApplicationSourceControl(configuration);

            var isMaster = await sourceControl.IsOnMaster();
            if (!isMaster)
            {
                Console.WriteLine("Not on master branch. Switching to master...");
                await sourceControl.Checkout(configuration.MasterBranchName, true);
            }

            var pullResult = await sourceControl.UpdateToBranchLatest(configuration.MasterBranchName);
            if (pullResult == SourceControl.SourceControlResult.Failure)
            {
                Console.WriteLine("Failed to pull latest.");
                return;
            }

            var buildEngine = new BuildEngine(configuration);
            await buildEngine.Execute();
        }
    }
}
