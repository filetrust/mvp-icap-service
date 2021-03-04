using Glasswall.IcapServer.CloudProxyApp.AdaptationService;
using Glasswall.IcapServer.CloudProxyApp.Configuration;
using Newtonsoft.Json;

namespace Glasswall.IcapServer.CloudProxyApp.Formatters
{
    public class JsonReturnConfigFormatter : IReturnConfigFormatter
    {
        public string Write(AdaptationRequestOutcome outcome)
        {
            string formattedConfig = string.Empty;
            if (outcome.OutcomeHeaders.Count > 0)
            {
                var formatData = new ReturnConfigData
                {
                    OutcomeHeaders = outcome.OutcomeHeaders
                };
                formattedConfig = JsonConvert.SerializeObject(formatData);
            }

            return formattedConfig;
        }
    }
}
