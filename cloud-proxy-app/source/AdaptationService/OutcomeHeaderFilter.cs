using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glasswall.IcapServer.CloudProxyApp.AdaptationService
{
    public class OutcomeHeaderFilter : IHeaderFilter
    {
        const string OutcomeHeaderKeyRoot = "outcome-header";
        private readonly ILogger<OutcomeHeaderFilter> logger;

        public OutcomeHeaderFilter(ILogger<OutcomeHeaderFilter> logger)
        {
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public IDictionary<string, string> Extract(IDictionary<string, object> headers)
        {
            var returnStore = new Dictionary<string, string>();

            if (headers?.Count > 0)
            {
                var outcomeHeaders = headers.Where(h => h.Key.StartsWith(OutcomeHeaderKeyRoot)).ToDictionary(p => p.Key, p => p.Value);

                var fileId = GetHeaderValue(headers["file-id"]);

                foreach (var item in outcomeHeaders)
                {
                    var strippedKey = item.Key.Remove(0, OutcomeHeaderKeyRoot.Length + 1);
                    var valueString = GetHeaderValue(item.Value);
                    if (string.IsNullOrEmpty(valueString))
                    {
                        logger.LogWarning($"FileId:{fileId}: Invalid outcome header value for {item.Key} ");
                        continue;
                    }
                    returnStore.Add(strippedKey, valueString);
                }
            }

            return returnStore;
        }

        private string GetHeaderValue(object value)
        {
            try
            {
                return Encoding.UTF8.GetString((byte[])value);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
