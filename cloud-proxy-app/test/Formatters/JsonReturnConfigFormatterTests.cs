using Glasswall.IcapServer.CloudProxyApp.AdaptationService;
using Glasswall.IcapServer.CloudProxyApp.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Glasswall.IcapServer.CloudProxyApp.Tests.Formatters
{
    class JsonReturnConfigFormatterTests
    {
        [Test]
        public void No_Supplied_Header_Returns_Empty_String()
        {
            // Arrange
            var testOutcome = new AdaptationRequestOutcome
            {
                OutcomeHeaders = new Dictionary<string, string>()
            };
            var formatter = new JsonReturnConfigFormatter();

            // Act
            var formattedData = formatter.Write(testOutcome);

            // Assert        
            Assert.That(formattedData, Is.Empty, "the formated data should be empty if no headers were provided");
        }

        [Test]
        public void Output_Is_Correctly_Formatted()
        {
            // Arrange
            var schemaGenerator = new JSchemaGenerator();
            var schema = schemaGenerator.Generate(typeof(ConfigData));

            var testOutcome = new AdaptationRequestOutcome
            {
                OutcomeHeaders = new Dictionary<string, string>
                {
                    {"first-header", "first-header-value" }
                }
            };
            var formatter = new JsonReturnConfigFormatter();

            // Act
            var formattedData = formatter.Write(testOutcome);

            // Assert

            JObject configData = JObject.Parse(formattedData);
            IList<string> parseErrors = new List<string>();
            Assert.That(configData.IsValid(schema, out parseErrors), Is.True, $"the formated data comply with the expected schema: {string.Join(",", parseErrors.A)}");
        }


    }

    class ConfigData
    {
        [Required]
        [JsonProperty(PropertyName = "outcome-headers")]
        public IDictionary<string, string> OutcomeHeaders;
    }
}
