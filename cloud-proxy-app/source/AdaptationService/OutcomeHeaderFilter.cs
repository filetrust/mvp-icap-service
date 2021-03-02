using System.Collections.Generic;

namespace Glasswall.IcapServer.CloudProxyApp.AdaptationService
{
    public class OutcomeHeaderFilter : IHeaderFilter
    {
        public IDictionary<string, string> Extract(IDictionary<string, object> headers)
        {
            var headerKey = "outcome-header-first-header";
            var returnStore =  new Dictionary<string, string>();

            if (headers.ContainsKey(headerKey))
            {
                returnStore.Add("first-header", headers[headerKey] as string);
            }

            return returnStore;
        }
    }
}
