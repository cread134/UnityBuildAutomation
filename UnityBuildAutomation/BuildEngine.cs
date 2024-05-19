using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityBuildAutomation
{
    public enum BuildResult { Success, Failure }
    internal class BuildEngine
    {
        private readonly Configuration configuration;
        public BuildEngine(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public async Task Execute()
        {
            Console.WriteLine("Executing build start");
            var buildResult = await RunUnityBuild();
            if (buildResult == BuildResult.Failure)
            {
                Console.WriteLine("Build failed.");
                return;
            }
            Console.WriteLine("Build succeeded.");
        }

        private async Task<BuildResult> RunUnityBuild()
        {
            var arguments = $"-projectPath {configuration.GetTargetRepoPath()} -quit -nographics -batchmode -executeMethod BuildPipe.Build";
            Console.WriteLine($"Running Unity with arguments: {arguments}");
            var processStartInfo = new ProcessStartInfo
            {
                FileName = configuration.UnityExecutablePath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var process = new Process { StartInfo = processStartInfo };
            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data == null)
                {
                    return;
                }
                Console.WriteLine(e.Data);
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data == null)
                {
                    return;
                }
                Console.WriteLine(e.Data);
            };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync();
            if (process.ExitCode != 0)
            {
                return BuildResult.Failure;
            }
            return BuildResult.Success;
        }
    }
}
