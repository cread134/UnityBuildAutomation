using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityBuildAutomation.SourceControl;

namespace UnityBuildAutomation
{
    public class ApplicationStartupDirector
    {
        public async Task StartupTask()
        {
            Console.WriteLine("Welcome to Unity Build Automation!");
            ManageConfiguration();
            var configuration = ConfigurationManager.LoadConfiguration();

            var sourceControl = new ApplicationSourceControl(configuration);
            Console.WriteLine($"Checking if repository exists...{configuration.RepositoryRootPath}");
            if (!sourceControl.DoesRepositoryExist())
            {
                Console.WriteLine("Repository does not exist. Cloning repository...");
                await sourceControl.CloneRepository();
            }
        }

        void ManageConfiguration()
        {
            var currentConfig = ConfigurationManager.LoadConfiguration();
            Console.WriteLine($"Current configuration:\n{currentConfig}");
            Console.WriteLine("Would you like to change the configuration? (y/n)");
            var response = Console.ReadLine() ?? string.Empty;
            if (response.ToLower() == "y")
            {
                Console.WriteLine("Enter the remote repository path:");
                var remoteRepositoryPath = Console.ReadLine() ?? string.Empty;
                Console.WriteLine("Enter the repository root path:");
                var repositoryRootPath = Console.ReadLine() ?? string.Empty;
                Console.WriteLine("Enter the build path:");
                var buildPath = Console.ReadLine() ?? string.Empty;

                var newConfig = new Configuration
                {
                    RemoteRepositoryPath = remoteRepositoryPath,
                    RepositoryRootPath = repositoryRootPath,
                    BuildPath = buildPath,
                };
                ConfigurationManager.SaveConfiguration(newConfig);
            }
        }
    }
}
