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
        public static bool DoesRepositoryExist()
        {
            return Directory.Exists(ConfigurationManager.RepositoryRootPath)
                && Directory.Exists(Path.Combine(ConfigurationManager.RepositoryRootPath, ".git"));
        }

        public async Task<SourceControlResult> CloneRepository()
        {
            if (Directory.Exists(ConfigurationManager.RepositoryRootPath))
            {
                Directory.CreateDirectory(ConfigurationManager.RepositoryRootPath);
            }

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = $"clone {ConfigurationManager.RemoteRepositoryPath} {ConfigurationManager.RepositoryRootPath}\\",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var process = new Process { StartInfo = processStartInfo };
            process.OutputDataReceived += OnOutputDataReceived;
            process.ErrorDataReceived += (sender, e) =>
            {
                OnErrorDataReceived(sender, e);
                throw new Exception(e.Data);
            };

            try
            {
                process.Start();
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return SourceControlResult.Failure;
            }
            return SourceControlResult.Success;
        }
        void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Data);
            Console.ResetColor();
        }

        void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }
    }
}
