using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glasswall.IcapServer.CloudProxyApp.Configuration
{
    public class ReturnConfigData
    {
        [JsonProperty(PropertyName = "outcome-headers")]
        public IDictionary<string, string> OutcomeHeaders { get; set; }
    }
}
