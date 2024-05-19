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
            Console.WriteLine($"Checking if repository exists...{configuration.RemoteRepositoryPath}");
            if (!sourceControl.DoesRepositoryExist())
            {
                Console.WriteLine("Repository does not exist. Cloning repository...");
                await sourceControl.CloneRepository();
            } else
            {
                Console.WriteLine("Repository exists.");
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
                Console.WriteLine("Config editor: \n (Press enter to leave as is)");
                Console.WriteLine("Enter the remote repository path:");
                var remoteRepositoryPath = Console.ReadLine() ?? currentConfig.RemoteRepositoryPath;
                Console.WriteLine("Enter the remote repository name:");
                var remoteRepositoryName = Console.ReadLine() ?? currentConfig.RemoteRepositoryName;
                Console.WriteLine("Enter the repository root directory:");
                var repositoryRootDirectory = Console.ReadLine() ?? currentConfig.RepositoryRootDirectory;
                Console.WriteLine("Enter master branch name:");
                var masterBranchName = Console.ReadLine() ?? currentConfig.MasterBranchName;
                Console.WriteLine("Enter Unity executable path:");
                var unityExecutablePath = Console.ReadLine() ?? currentConfig.UnityExecutablePath;

                var newConfig = new Configuration
                {
                    RemoteRepositoryPath = remoteRepositoryPath,
                    RepositoryRootDirectory = repositoryRootDirectory,
                    RemoteRepositoryName = remoteRepositoryName,
                    MasterBranchName = masterBranchName,
                    UnityExecutablePath = unityExecutablePath
                };
                ConfigurationManager.SaveConfiguration(newConfig);
            }
            Console.WriteLine("Configuration saved.");
            Console.WriteLine("Checking configuration validity...");
            var valid = IsConfigurationValid(currentConfig);
            if (!valid)
            {
                Console.WriteLine("Configuration is invalid. Please manually update config file at " + ConfigurationManager.ConfigurationPath);
                Environment.Exit(1);
            }
            Console.WriteLine("Configuration is valid.");
        }

        bool IsConfigurationValid(Configuration configuration)
        {
            var stringNotInvalid = !string.IsNullOrEmpty(configuration.RemoteRepositoryPath) &&
                !string.IsNullOrEmpty(configuration.RemoteRepositoryName) &&
                !string.IsNullOrEmpty(configuration.RepositoryRootDirectory) &&
                !string.IsNullOrEmpty(configuration.MasterBranchName) &&
                !string.IsNullOrEmpty(configuration.UnityExecutablePath);
            if (!stringNotInvalid)
            {
                Console.WriteLine("Config cannot have empty arguments");
                return false;
            }
            if (!File.Exists(configuration.UnityExecutablePath))
            {
                Console.WriteLine("Unity executable path does not exist. Verify you have the correct install!");
                return false;
            }
            return true;
        }
    }
}
