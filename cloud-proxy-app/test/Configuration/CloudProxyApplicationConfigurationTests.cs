using Glasswall.IcapServer.CloudProxyApp.Configuration;
using NUnit.Framework;

namespace Glasswall.IcapServer.CloudProxyApp.Tests.Configuration
{
    class CloudProxyApplicationConfigurationTests
    {
        CloudProxyApplicationConfiguration configuration;

        [SetUp]
        public void SetUp()
        {
            configuration = new CloudProxyApplicationConfiguration();
        }

        [Test]
        public void FileId_is_correctly_mapped_from_commandline()
        {
            Assert.That(nameof(configuration.FileId), Is.EqualTo(CommandLineSwitchMapping.FileIdConfigurationKey), "FileId Property and mapping key must be the same");
        }

        [Test]
        public void InputFilepath_is_correctly_mapped_from_commandline()
        {
            Assert.That(nameof(configuration.InputFilepath), Is.EqualTo(CommandLineSwitchMapping.InputConfigurationKey), "InputFilepath Property and mapping key must be the same");
        }

        [Test]
        public void OutputFilepath_is_correctly_mapped_from_commandline()
        {
            Assert.That(nameof(configuration.OutputFilepath), Is.EqualTo(CommandLineSwitchMapping.OutputConfigurationKey), "OutputFilepath Property and mapping key must be the same");
        }

        [Test]
        public void ReturnConfigFilepath_is_correctly_mapped_from_commandline()
        {
            Assert.That(nameof(configuration.ReturnConfigFilepath), Is.EqualTo(CommandLineSwitchMapping.ReturnConfigurationKey), "ReturnConfigFilepath Property and mapping key must be the same");
        }
    }
}
