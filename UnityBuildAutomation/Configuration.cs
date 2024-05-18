namespace UnityBuildAutomation
{
    public class Configuration
    {
        public override string ToString()
        {
            return typeof(Configuration).GetProperties()
                .Select(property => $"{property.Name}: {property.GetValue(this)}")
                .Aggregate((current, next) => $"{current}, {next}");
        }
        public string RemoteRepositoryPath { get; set; } = string.Empty;
        public string RemoteRepositoryName { get; set; } = string.Empty;
        public string RepositoryRootDirectory { get; set; } = string.Empty;
        public string RepositoryRootPath { get; set; } = string.Empty;
        public string? BuildPath { get; internal set; }
    }
}
