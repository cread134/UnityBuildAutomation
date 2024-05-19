using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityBuildAutomation.SourceControl
{
    public enum SourceControlResult { Success, Failure }
    internal class ApplicationSourceControl
    {
        private readonly Configuration configuration;

        public ApplicationSourceControl(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public bool DoesRepositoryExist()
        {
            if (!Directory.Exists(configuration.GetTargetRepoPath()))
            {
                return false;
            }
            return File.Exists(Path.Combine(configuration.GetTargetRepoPath(), ".gitattributes"));
        }

        async Task<(SourceControlResult sourceControlResult, List<string> outputResults)> RunGitProcess(string arguments)
        {
            var outputResults = new List<string>();
            Console.WriteLine($"Running git {arguments}");
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "git",
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
                OnOutputDataReceived(sender, e);
                outputResults.Add(e.Data);
            };

            bool errror = false;
            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data == null)
                {
                    return;
                }
                OnOutputDataReceived(sender, e);
                outputResults.Add(e.Data);
            };

            try
            {
                process.Start();
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();
                if (process.ExitCode != 0)
                {
                    return (SourceControlResult.Failure, outputResults);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return (SourceControlResult.Failure, outputResults);
            }
            return (SourceControlResult.Success, outputResults);
        }

        public async Task<SourceControlResult> CloneRepository()
        {
            if (Directory.Exists(configuration.GetTargetRepoPath()))
            {
                Directory.CreateDirectory(configuration.GetTargetRepoPath());
            }

            var args = $"clone {configuration.RemoteRepositoryPath} {configuration.GetTargetRepoPath()}/";
            var result = await RunGitProcess(args);
            return result.sourceControlResult;
        }

        void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        public async Task<bool> IsOnMaster()
        {
            var arguments = $"-C \"{configuration.GetTargetRepoPath()}\" rev-parse --abbrev-ref HEAD";
            var result = await RunGitProcess(arguments);
            if (result.sourceControlResult == SourceControlResult.Failure)
            {
                return false;
            }
            if (result.outputResults.Count == 0)
            {
                return false;
            }
            return result.outputResults.FirstOrDefault() == configuration.MasterBranchName;
        }

        public async Task<SourceControlResult> Checkout(string branchName, bool ignoredChanges = false)
        {
            var arguments = $"-C {configuration.GetTargetRepoPath()} checkout {branchName}";
            if (ignoredChanges)
            {
                arguments += " -f";
            }
            var result = await RunGitProcess(arguments);
            return result.sourceControlResult;
        }

        internal async Task<SourceControlResult> Prune()
        {
            var arguments = $"-C \"{configuration.GetTargetRepoPath()}\" remote prune origin";
            return (await RunGitProcess(arguments)).sourceControlResult;
        }

        internal async Task<SourceControlResult> UpdateToBranchLatest(string branchName)
        {
            var pruneResult = await Prune();
            if (pruneResult == SourceControlResult.Failure)
            {
                return SourceControlResult.Failure;
            }
            Thread.Sleep(1000);
            var fetchResult = await Fetch(branchName);
            if (fetchResult == SourceControlResult.Failure)
            {
                return SourceControlResult.Failure;
            }
            Thread.Sleep(1000);
            return await ResetToHead(branchName);
        }
        internal async Task<SourceControlResult> Pull(string branchName)
        {
            var arguments = $"-C \"{configuration.GetTargetRepoPath()}\" pull";
            var result = await RunGitProcess(arguments);
            return result.sourceControlResult;
        }

        internal async Task<SourceControlResult> ResetToHead(string branchName)
        {
            var arguments = $"-C \"{configuration.GetTargetRepoPath()}\" reset --hard origin/{branchName}";
            var result = await RunGitProcess(arguments);
            return result.sourceControlResult;
        }

        internal async Task<SourceControlResult> Fetch(string branchName)
        {
            var arguments = $"-C \"{configuration.GetTargetRepoPath()}\" fetch origin {branchName}";
            var result = await RunGitProcess(arguments);
            return result.sourceControlResult;
        }
    }
}
