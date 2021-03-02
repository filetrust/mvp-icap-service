using System.Collections.Generic;
using System.Linq;

namespace Glasswall.IcapServer.CloudProxyApp.AdaptationService
{
    public class OutcomeHeaderFilter : IHeaderFilter
    {
        const string OutcomeHeaderKeyRoot = "outcome-header";

        public IDictionary<string, string> Extract(IDictionary<string, object> headers)
        {
            var outcomeHeaders = headers.Where(h => h.Key.StartsWith(OutcomeHeaderKeyRoot)).ToDictionary(p => p.Key, p => p.Value); 
            var returnStore =  new Dictionary<string, string>();
            
            foreach (var item in outcomeHeaders)
            {
                var strippedKey = item.Key.Remove(0, OutcomeHeaderKeyRoot.Length + 1);
                var valueString = item.Value as string;
                returnStore.Add(strippedKey, valueString);
            }

            return returnStore;
        }
    }
}
