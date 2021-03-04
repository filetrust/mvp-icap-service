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
            var schema = GenerateReturnConfigSchema();

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
            Assert.That(configData.IsValid(schema, out IList<string> parseErrors), Is.True, $"the formated data comply with the expected schema: {string.Join(",", parseErrors)}");
        }

        private JSchema GenerateReturnConfigSchema()
        {
            string schemaJson = @"{
              'type': 'object',
              'properties': {
                  'outcome-headers': {
                      'type': 'object',
                      'additionalProperties': {
                          'type': [
                              'string',
                              'null'
                          ]
                      }
                  }
              },
              'required': [
                'outcome-headers'
              ]
            }";
            return JSchema.Parse(schemaJson);
        }
    }
}
