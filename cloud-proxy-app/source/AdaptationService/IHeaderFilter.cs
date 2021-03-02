using System.Collections.Generic;

namespace Glasswall.IcapServer.CloudProxyApp.AdaptationService
{
    public interface IHeaderFilter
    {
        IDictionary<string, string> Extract(IDictionary<string, object> headers);
    }
}
