namespace NServiceBus.Config.Yaml
{
    using System.IO;

    public static class EndpointConfigurationExtensions
    {
        public static void FromYaml(this EndpointConfiguration cfg, string path)
        {
            var worker = new Configurer(cfg);
            worker.ApplyYaml(new StreamReader(path));
        }

        public static void FromYamlString(this EndpointConfiguration cfg, string yamlString)
        {
            var worker = new Configurer(cfg);
            worker.ApplyYaml(new StringReader(yamlString));
        }
    }
}
