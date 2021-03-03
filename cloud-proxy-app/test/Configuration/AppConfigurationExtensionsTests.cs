using Glasswall.IcapServer.CloudProxyApp.Configuration;
using Moq;
using NUnit.Framework;

namespace Glasswall.IcapServer.CloudProxyApp.Tests.Configuration
{
    class AppConfigurationExtensionsTests
    {
        [Test]
        public void ReturnConfigFilePathSpecified_returns_true_if_filepath_provided()
        {
            // Arrange
            const string validFilepath = "C:\test\returnconfig.json";
            Mock<IAppConfiguration> mockAppConfiguration = new Mock<IAppConfiguration>();
            mockAppConfiguration.SetupGet(m => m.ReturnConfigFilepath).Returns(validFilepath);
            var appConfiguration = mockAppConfiguration.Object;

            // Act
            var output = appConfiguration.ReturnConfigFilePathSpecified();

            // Assert
            Assert.That(output, Is.True, "Extension should return false if no path provided");
        }

        [Test]
        public void ReturnConfigFilePathSpecified_returns_false_if_no_filepath_is_provided([Values("", null)] string invalidFilepath)
        {
            // Arrange
            Mock<IAppConfiguration> mockAppConfiguration = new Mock<IAppConfiguration>();
            mockAppConfiguration.SetupGet(m => m.ReturnConfigFilepath).Returns(invalidFilepath);
            var appConfiguration = mockAppConfiguration.Object;

            // Act
            var output = appConfiguration.ReturnConfigFilePathSpecified();

            // Assert
            Assert.That(output, Is.False, "Extension should return false if no path provided");
        }
    }
}
