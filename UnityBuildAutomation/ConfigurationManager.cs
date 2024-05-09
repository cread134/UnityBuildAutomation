using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace UnityBuildAutomation
{
    internal class ConfigurationManager
    {
        public const string RemoteRepositoryPath = "https://github.com/cread134/DSA_Assignment1";
        public const string RemoteRepositoryName = "PDT_2023_ArmlessSamurai";

        public static string RepositoryRootDirectory => Path.Combine("C:", "git", "UnityBuildAutomation", "source");
        public static string RepositoryRootPath => Path.Combine(RepositoryRootDirectory, RemoteRepositoryName);
    }
}
