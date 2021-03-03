using Glasswall.IcapServer.CloudProxyApp.Configuration;
using NUnit.Framework;

namespace Glasswall.IcapServer.CloudProxyApp.Tests.Configuration
{
    public class CloudProxyApplicationConfigurationCheckerTests
    {
        readonly string TestInputPath = "c:/test/inputpath.txt";
        readonly string TestOutputPath = "c:/test/outputpath.txt";
        const string TestReturnConfigPath = "c:/test/returnpath.txt";

        private CloudProxyApplicationConfigurationChecker checkerUnderTest;

        [SetUp]
        public void Setup()
        {
            checkerUnderTest = new CloudProxyApplicationConfigurationChecker();
        }

        [Test]
        public void CheckConfiguration_reports_missing_outputpath_configuration()
        {
            // Arrange
            var config = new CloudProxyApplicationConfiguration
            {
                InputFilepath = TestInputPath
            };

            // Act
            var exception = Assert.Throws<InvalidApplicationConfigurationException>(delegate { checkerUnderTest.CheckConfiguration(config); }, "Expected an exception to be thrown due to missing configuration");
            // Assert
            Assert.That(exception.Message.Contains("OutputFilepath"), "Expected missing output filepath to be reported");
        }

        [Test]
        public void CheckConfiguration_reports_missing_inputpath_configuration()
        {
            // Arrange
            var config = new CloudProxyApplicationConfiguration
            {
                OutputFilepath = TestOutputPath
            };

            // Act
            var exception = Assert.Throws<InvalidApplicationConfigurationException>(delegate { checkerUnderTest.CheckConfiguration(config); }, "Expected an exception to be thrown due to missing configuration");
            // Assert
            Assert.That(exception.Message.Contains("InputFilepath"), "Expected missing output filepath to be reported");
        }

        [Test]
        public void CheckConfiguration_reports_all_missing_configuration()
        {
            // Arrange
            var config = new CloudProxyApplicationConfiguration();

            // Act
            var exception = Assert.Throws<InvalidApplicationConfigurationException>(delegate { checkerUnderTest.CheckConfiguration(config); }, "Expected an exception to be thrown due to missing configuration");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(exception.Message.Contains("InputFilepath"), "Expected missing output filepath to be reported");
                Assert.That(exception.Message.Contains("OutputFilepath"), "Expected missing output filepath to be reported");
            });
        }

        [Test]
        public void CheckConfiguration_valid_configuration_with_optional_returnconfig([Values("", TestReturnConfigPath, null)] string returnConfigPath)
        {
            // Arrange
            var config = new CloudProxyApplicationConfiguration
            {
                OutputFilepath = TestOutputPath,
                InputFilepath = TestInputPath,
                ReturnConfigFilepath = returnConfigPath
            };

            // Assert
            Assert.DoesNotThrow(delegate { checkerUnderTest.CheckConfiguration(config); }, "No exception expected with correct configuration");
        }
    }
}