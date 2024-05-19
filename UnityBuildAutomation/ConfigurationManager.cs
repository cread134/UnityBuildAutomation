using Newtonsoft.Json;

namespace UnityBuildAutomation
{
    internal class ConfigurationManager
    {
        public static string ConfigurationPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configuration.json");
        public static Configuration LoadConfiguration()
        {
            if (!File.Exists(ConfigurationPath))
            {
                throw new FileNotFoundException("Configuration file not found.", ConfigurationPath);
            }

            var configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(ConfigurationPath));
            if (configuration == null)
            {
                throw new Exception("Configuration file is empty.");
            }
            return configuration;
        }

        public static void SaveConfiguration(Configuration configuration)
        {
            File.WriteAllText(ConfigurationPath, JsonConvert.SerializeObject(configuration, Formatting.Indented));
        }
    }
}
