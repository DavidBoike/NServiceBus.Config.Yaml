﻿namespace NServiceBus.Config.Yaml.Tests
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

            test.Yaml = @"AuditProcessedMessagesTo: the-accountant";

            test.Run();
        }
    }
}
