using System.Collections.Generic;

namespace Glasswall.IcapServer.CloudProxyApp.AdaptationService
{
    interface IHeaderFilter
    {
        IDictionary<string, string> Extract(IDictionary<string, object> headers);
    }
}
