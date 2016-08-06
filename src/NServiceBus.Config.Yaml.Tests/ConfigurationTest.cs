namespace NServiceBus.Config.Yaml.Tests
{
    using System;
    using System.Collections.Concurrent;
    using KellermanSoftware.CompareNetObjects;
    using System.Linq;
    using System.Reflection;
    using Configuration.AdvanceExtensibility;
    using NUnit.Framework;
    using Settings;

    public class ConfigurationTest
    {
        public ConfigurationTest()
            : this("FakeEndpointName")
        {  
        }

        public ConfigurationTest(string endpointName)
        {
            this.EndpointName = endpointName;

            CompareConfig = new ComparisonConfig();
            CompareConfig.ComparePrivateFields = true;
            CompareConfig.CompareProperties = false;
            CompareConfig.CompareReadOnly = true; // default
            CompareConfig.CompareStaticProperties = false;
            CompareConfig.Caching = true; // default
            CompareConfig.AutoClearCache = false;
        }

        public string EndpointName { get; }
        public string Yaml { get; set; }
        public Action<EndpointConfiguration> Common { get; set; }
        public Action<EndpointConfiguration> Control { get; set; }
        public ComparisonConfig CompareConfig { get; }

        public void Run()
        {
            compareLogic = new CompareLogic(CompareConfig);

            var controlCfg = new EndpointConfiguration(EndpointName);
            var testCfg = new EndpointConfiguration(EndpointName);

            if (Common != null)
            {
                Common(controlCfg);
                Common(testCfg);

            }

            if (Control != null)
            {
                Control(controlCfg);
            }
            if (Yaml != null)
            {
                testCfg.FromYamlString(Yaml);
            }
           
            var controlSettings = GetMergedSettings(controlCfg.GetSettings());
            var testSettings = GetMergedSettings(testCfg.GetSettings());

            AssertSettingsEquality(controlSettings, testSettings);
        }

        ConcurrentDictionary<string, object> GetMergedSettings(SettingsHolder settings)
        {
            var privateBindings = BindingFlags.GetField | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic;

            var defaults = typeof(SettingsHolder).GetField("Defaults", privateBindings)?.GetValue(settings) as ConcurrentDictionary<string, object>;
            var overrides = typeof(SettingsHolder).GetField("Overrides", privateBindings)?.GetValue(settings) as ConcurrentDictionary<string, object>;
            var result = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            var allkeys = defaults.Keys.Union(overrides.Keys);

            foreach (var key in allkeys)
            {
                object value;
                if (overrides.TryGetValue(key, out value) || defaults.TryGetValue(key, out value))
                {
                    result[key] = value;
                }
            }
            return result;
        }

        void AssertSettingsEquality(ConcurrentDictionary<string, object> control, ConcurrentDictionary<string, object> test)
        {
            var allKeys = control.Keys.Union(test.Keys);

            foreach (var key in allKeys)
            {
                Assert.IsTrue(control.ContainsKey(key), "Test settings contained key `{0}` not found in control.", key);
                Assert.IsTrue(test.ContainsKey(key), "Test settings missing key `{0}` present in control.", key);

                AssertValueEquality(key, control[key], test[key]);
            }
        }

        void AssertValueEquality(string key, object control, object test)
        {
            var controlType = control.GetType();
            var testType = test.GetType();

            Assert.AreEqual(controlType, testType, "Mismatched value types for key `{0}`: `{1}` vs. `{2}`", key, controlType, testType);

            Console.WriteLine($"Comparing values: {key} ({controlType})");
            if (controlType.IsValueType)
            {
                Assert.AreEqual(control, test);
            }
            //else if (typeof(IComparable).IsAssignableFrom(controlType))
            //{
            //    Assert.AreEqual(0, ((IComparable)control).CompareTo(test));
            //}
            else
            {
                var compareResult = compareLogic.Compare(control, test);
                Assert.IsTrue(compareResult.AreEqual, compareResult.DifferencesString);
            }
        }

        CompareLogic compareLogic;
    }
}
