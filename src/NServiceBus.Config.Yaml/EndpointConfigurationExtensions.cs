namespace NServiceBus.Config.Yaml
{
    using System.IO;
    using YamlDotNet.RepresentationModel;

    public static class EndpointConfigurationExtensions
    {
        public static void FromYaml(this EndpointConfiguration cfg, string path)
        {
            using (var reader = new StreamReader(path))
            {
                var yaml = new YamlStream();
                yaml.Load(reader);
                Apply(cfg, yaml);
            }
        }

        public static void FromYamlString(this EndpointConfiguration cfg, string yamlString)
        {
            using (var reader = new StringReader(yamlString))
            {
                var yaml = new YamlStream();
                yaml.Load(reader);
                Apply(cfg, yaml);
            }
        }

        static void Apply(EndpointConfiguration cfg, YamlStream yaml)
        {
            
        }
    }
}
