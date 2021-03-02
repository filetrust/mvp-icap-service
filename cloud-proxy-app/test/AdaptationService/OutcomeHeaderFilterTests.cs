using Glasswall.IcapServer.CloudProxyApp.AdaptationService;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Glasswall.IcapServer.CloudProxyApp.Tests.AdaptationService
{
    public class OutcomeHeaderFilterTests
    {
        [Test]
        public void Empty_Dictionary_If_No_Properties()
        {
            // Arrange
            var filter = new OutcomeHeaderFilter();

            // Act
            var output = filter.Extract(new Dictionary<string, object>());

            // Assert
            Assert.That(output, Is.Empty, "Outcome Header filter should return an empty list of headers if the input is empty");
        }

        [Test]
        public void Empty_Dictionary_If_No_Outcome_Headers()
        {
            // Arrange
            var filter = new OutcomeHeaderFilter();
            var properties = new Dictionary<string, object>()
            {
                { "file-id", Guid.NewGuid() },
                { "file-outcome", "replace"}
            };

            // Act
            var output = filter.Extract(properties);

            // Assert
            Assert.That(output, Is.Empty, "Outcome Header filter should return an empty list of headers if the input only has unrelated properties");
        }
    }
}
