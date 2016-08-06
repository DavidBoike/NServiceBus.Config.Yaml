namespace NServiceBus.Config.Yaml.Tests
{
    using System;
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

            test.Yaml = @"SendFailedMessagesTo: ironman";

            test.Run();
        }

        [Test]
        public void CanUseSingleQuotes()
        {
            var test = new ConfigurationTest();

            test.Control = cfg =>
            {
                cfg.SendFailedMessagesTo("ironman");
            };

            test.Yaml = @"SendFailedMessagesTo: 'ironman'";

            test.Run();
        }

        [Test]
        public void CanUseDoubleQuotes()
        {
            var test = new ConfigurationTest();

            test.Control = cfg =>
            {
                cfg.SendFailedMessagesTo("ironman");
            };

            test.Yaml = "SendFailedMessagesTo: \"ironman\"";

            test.Run();
        }

        [Test]
        public void CanNotUseBackticks()
        {
            var test = new ConfigurationTest();

            test.Control = cfg =>
            {
                cfg.SendFailedMessagesTo("ironman");
            };

            test.Yaml = "SendFailedMessagesTo: `ironman`";

            Assert.Throws<YamlDotNet.Core.SyntaxErrorException>(() => test.Run());
        }

        [Test]
        public void CanSetAuditQueue()
        {
            var test = new ConfigurationTest();

            test.Control = cfg =>
            {
                cfg.AuditProcessedMessagesTo("the-accountant");
            };

            test.Yaml = @"AuditProcessedMessagesTo: the-accountant";

            test.Run();
        }

        [Test]
        public void CanSetAuditQueueWithTTBR()
        {
            var test = new ConfigurationTest();

            test.Control = cfg =>
            {
                cfg.AuditProcessedMessagesTo("the-accountant", TimeSpan.FromDays(1));
            };

            test.Yaml = @"AuditProcessedMessagesTo: ['the-accountant', '1.00:00:00']";

            test.Run();
        }

        [Test]
        public void CanUseYamlComments()
        {
            var test = new ConfigurationTest();

            test.Control = cfg =>
            {
                cfg.SendFailedMessagesTo("error");
            };

            test.Yaml = @"---
# this is a comment
SendFailedMessagesTo: error
#this is another comment






#and another comment after blank lines
---";
            test.Run();
        }
    }
}
