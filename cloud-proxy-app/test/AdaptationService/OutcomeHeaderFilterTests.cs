using Glasswall.IcapServer.CloudProxyApp.AdaptationService;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Glasswall.IcapServer.CloudProxyApp.Tests.AdaptationService
{
    public class OutcomeHeaderFilterTests
    {
        Dictionary<string, object> properties;
        Mock<ILogger<OutcomeHeaderFilter>> mockLogger;
        OutcomeHeaderFilter outcomeHeaderFilter;

        [SetUp]
        public void Init()
        {
            properties = new Dictionary<string, object>();
            mockLogger = new Mock<ILogger<OutcomeHeaderFilter>>();
            outcomeHeaderFilter = new OutcomeHeaderFilter(mockLogger.Object);
        }

        [Test]
        public void Empty_Dictionary_If_No_Properties()
        {
            // Arrange

            // Act
            var output = outcomeHeaderFilter.Extract(properties);

            // Assert
            Assert.That(output, Is.Empty, "Outcome Header filter should return an empty list of headers if the input is empty");
        }

        [Test]
        public void Empty_Dictionary_If_No_Outcome_Headers()
        {
            // Arrange
            AddStandardProperties(properties);
       
            // Act
            var output = outcomeHeaderFilter.Extract(properties);

            // Assert
            Assert.That(output, Is.Empty, "Outcome Header filter should return an empty list of headers if the input only has unrelated properties");
        }

        [Test]
        public void Single_Header_Is_Returned()
        {
            AddStandardProperties(properties);
            properties.Add("outcome-header-first-header", "First Header Value");
   
            // Act
            var output = outcomeHeaderFilter.Extract(properties);

            // Assert
            Assert.That(output.Count, Is.EqualTo(1), "Outcome Header filter should return a single outcome-header entry");
            Assert.That(output, Contains.Key("first-header"), "Outcome Header filter extract the correct key");
            Assert.That(output["first-header"], Is.EqualTo("First Header Value"), "Outcome Header filter should extract the matching value");
        }

        [Test]
        public void Multiple_Headers_Are_Returned()
        {
            AddStandardProperties(properties);
            properties.Add("outcome-header-first-header", "First Header Value");
            properties.Add("outcome-header-second-header", "Second Header Value");

            // Act
            var output = outcomeHeaderFilter.Extract(properties);

            // Assert
            Assert.That(output.Count, Is.EqualTo(2), "Outcome Header filter should return both outcome-header entries");
            Assert.That(output, Contains.Key("second-header"), "Outcome Header filter extract the correct second key");
            Assert.That(output["second-header"], Is.EqualTo("Second Header Value"), "Outcome Header filter should extract the matching second value");
        }

        [Test]
        public void Numerical_Values_Not_Permitted_In_OutcomeHeaders()
        {
            var answer = 42;
            AddStandardProperties(properties);
            properties.Add("outcome-header-numeric-header", answer);

            // Act
            var output = outcomeHeaderFilter.Extract(properties);

            // Assert
            Assert.That(output.Count, Is.EqualTo(0), "Outcome Header filter should not return numerical entry value");
        }

        [Test]
        public void Invalid_Header_String_Not_Included([Values("", null)] string invalidValue)
        {
            AddStandardProperties(properties);
            properties.Add("outcome-header-first-header", "Valid Header Value");
            properties.Add("outcome-header-second-header", invalidValue);

            // Act
            var output = outcomeHeaderFilter.Extract(properties);

            // Assert
            Assert.That(output.Count, Is.EqualTo(1), "Outcome Header filter should return only valid header string values");
        }

        private void AddStandardProperties(IDictionary<string, object> properties)
        {
            properties.Add("file-id", Guid.NewGuid());
            properties.Add("file-outcome", "replace");
        }
    }
}
