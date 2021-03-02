using Glasswall.IcapServer.CloudProxyApp.AdaptationService;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace Glasswall.IcapServer.CloudProxyApp.Tests.AdaptationService
{
    public class OutcomeHeaderFilterTests
    {
        [Test]
        public void Empty_Dictionary_If_No_Outcome_Headers()
        {
            // Arrange
            var filter = new OutcomeHeaderFilter();

            // Act
            var output = filter.Extract(new Dictionary<string, object>());

            // Assert
            Assert.That(output, Is.Empty, "Outcome HEader filter should return an empty list of headers if the input is empty");

        }
    }
}
