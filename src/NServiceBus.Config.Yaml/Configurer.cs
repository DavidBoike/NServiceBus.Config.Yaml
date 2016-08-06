namespace NServiceBus.Config.Yaml
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using YamlDotNet.RepresentationModel;

    class Configurer
    {
        EndpointConfiguration cfg;
        Dictionary<Type, Dictionary<string, MethodInfo>> extensionMethods;

        public Configurer(EndpointConfiguration cfg)
        {
            this.cfg = cfg;

            extensionMethods = new Dictionary<Type, Dictionary<string, MethodInfo>>();
        }

        public void ApplyYaml(TextReader reader)
        {
            using (reader)
            {
                var yaml = new YamlStream();
                yaml.Load(reader);
                Apply(cfg, yaml);
            }
        }

        void Apply(EndpointConfiguration cfg, YamlStream yaml)
        {
            var yamlRoot = (YamlMappingNode)yaml.Documents[0].RootNode;
            foreach (var pair in yamlRoot.Children)
            {
                if (pair.Key.NodeType == YamlNodeType.Scalar)
                {
                    var keyNode = (YamlScalarNode)pair.Key;
                    var key = keyNode.Value;
                    Apply(cfg, key, pair.Value);
                }
                else
                {
                    throw new Exception("Don't know what to do when key isn't a scalar...");
                }
            }
        }

        void Apply(EndpointConfiguration cfg, string key, YamlNode value)
        {
            var method = GetExtensionMethod(typeof(EndpointConfiguration), key);
            if (method != null)
            {
                var parameters = method.GetParameters();
                var paramValues = new object[parameters.Length];
                paramValues[0] = cfg;
                paramValues[1] = ((YamlScalarNode) value).Value;
                method.Invoke(null, paramValues);
            }
            else
            {
                throw new Exception("Couldn't find an extension method");
            }
        }

        MethodInfo GetExtensionMethod(Type target, string methodName)
        {
            Dictionary<string, MethodInfo> methods;
            if (!extensionMethods.TryGetValue(target, out methods))
            {
                methods = new Dictionary<string, MethodInfo>();

                foreach (var assem in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var type in assem.GetTypes())
                    {
                        if (type.IsSealed && !type.IsGenericType && !type.IsNested && type.IsDefined(typeof(ExtensionAttribute), false))
                        {
                            foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
                            {
                                if (method.IsDefined(typeof(ExtensionAttribute), false))
                                {
                                    var parameters = method.GetParameters();
                                    if (parameters[0].ParameterType == target)
                                    {
                                        methods[method.Name] = method;
                                    }
                                }
                            }
                        }
                    }
                }

                extensionMethods[target] = methods;
            }

            MethodInfo lookupMethod;
            if (methods.TryGetValue(methodName, out lookupMethod))
            {
                return lookupMethod;
            }
    
            return null;
        }
    }
}
