namespace UnityBuildAutomation
{
    public class Configuration
    {
        public override string ToString()
        {
            return typeof(Configuration).GetProperties()
                .Select(property => $"{property.Name}: {property.GetValue(this)}")
                .Aggregate((current, next) => $"{current}\n{next}");
        }
        public string RemoteRepositoryPath { get; set; } = string.Empty;
        public string RemoteRepositoryName { get; set; } = string.Empty;
        public string RepositoryRootDirectory { get; set; } = string.Empty;
        public string MasterBranchName { get; set; } = string.Empty;
        public string UnityExecutablePath { get; set; } = string.Empty;

        public string GetTargetRepoPath() => Path.Combine(Path.GetFullPath(RepositoryRootDirectory), RemoteRepositoryName);
    }
}
