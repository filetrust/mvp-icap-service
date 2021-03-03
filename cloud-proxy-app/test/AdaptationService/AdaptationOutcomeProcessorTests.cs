using Glasswall.IcapServer.CloudProxyApp.AdaptationService;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;

namespace Glasswall.IcapServer.CloudProxyApp.Tests.AdaptationService
{
    public class AdaptationOutcomeProcessorTests
    {
        AdaptationOutcomeProcessor processor;

        [SetUp]
        public void SetUp()
        {
            var mockHeaderFilter = new Mock<IHeaderFilter>();
            mockHeaderFilter.Setup(m => m.Extract(It.IsAny<IDictionary<string, object>>()))
                .Returns((IDictionary<string, object> i) => new Dictionary<string, string>());
            processor = new AdaptationOutcomeProcessor(Mock.Of<ILogger<AdaptationOutcomeProcessor>>(), mockHeaderFilter.Object);
        }

        [Test]
        public void Replace_Return_Rebuilt_Outcome()
        {
            // Arrange
            IDictionary<string, object> headerMap = new Dictionary<string, object>
                    {
                        { "file-id", Encoding.UTF8.GetBytes("737ba1cc-492c-4292-9a2c-fc7bfc722dc6") },
                        { "file-outcome", Encoding.UTF8.GetBytes("replace") }
                    };

            // Act
            var result = processor.Process(headerMap, null);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(ReturnOutcome.GW_REBUILT), "expected the outcome to be 'rebuilt'");
        }

        [Test]
        public void Unmodified_Return_Unprocessed_Outcome()
        {
            // Arrange
            IDictionary<string, object> headerMap = new Dictionary<string, object>
                    {
                        { "file-id", Encoding.UTF8.GetBytes("737ba1cc-492c-4292-9a2c-fc7bfc722dc6") },
                        { "file-outcome", Encoding.UTF8.GetBytes("unmodified") },
                    };

            // Act
            var result = processor.Process(headerMap, null);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(ReturnOutcome.GW_UNPROCESSED), "expected the outcome to be 'unprocessed'");
        }

        [Test]
        public void Failed_Return_Failed_Outcome()
        {
            // Arrange
            IDictionary<string, object> headerMap = new Dictionary<string, object>
                    {
                        { "file-id", Encoding.UTF8.GetBytes("737ba1cc-492c-4292-9a2c-fc7bfc722dc6") },
                        { "file-outcome", Encoding.UTF8.GetBytes("failed") },
                    };

            // Act
            var result = processor.Process(headerMap, null);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(ReturnOutcome.GW_FAILED), "expected the outcome to be 'failed'");
        }

        [Test]
        public void Incorrect_Message_Error_Outcome()
        {
            // Arrange
            IDictionary<string, object> headerMap = new Dictionary<string, object>();

            // Act
            var result = processor.Process(headerMap, null);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(ReturnOutcome.GW_ERROR), "expected the outcome to be 'error'");
        }
    }
}
