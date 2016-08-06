namespace NServiceBus.Config.Yaml.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class Simple
    {

        [Test]
        public void NullCustomizationsShouldMatch()
        {
            var test = new ConfigurationTest();

            test.Run();
        }

        [Test]
        public void EmptyCustomizationsShouldMatch()
        {
            var test = new ConfigurationTest();

            test.Common = cfg => { };
            test.Control = cfg => { };

            test.Run();
        }

        [Test]
        public void CanSetErrorQueue()
        {
            var test = new ConfigurationTest();

            test.Control = cfg =>
            {
                cfg.SendFailedMessagesTo("ironman");
            };

            test.Yaml = @"---
SendFailedMessagesTo: ironman
---
";

            test.Run();
        }
    }
}
