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
            while (true)
            {
                Console.WriteLine("Welcome to Unity Build Automation!");
                Console.WriteLine($"Checking if repository exists...{ConfigurationManager.RepositoryRootPath}");
                if (!ApplicationSourceControl.DoesRepositoryExist())
                {
                    Console.WriteLine("Repository does not exist. Cloning repository...");
                    var sourceControl = new ApplicationSourceControl();
                    await sourceControl.CloneRepository();
                }
            }
        }
    }
}
