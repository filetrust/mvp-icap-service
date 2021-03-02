using System;
using System.Collections.Generic;

namespace Glasswall.IcapServer.CloudProxyApp.AdaptationService
{
    public class OutcomeHeaderFilter : IHeaderFilter
    {
        public IDictionary<string, string> Extract(IDictionary<string, object> headers)
        {
            return new Dictionary<string, string>();
        }
    }
}
