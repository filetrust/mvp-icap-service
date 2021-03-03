using System.Collections.Generic;

namespace Glasswall.IcapServer.CloudProxyApp.AdaptationService
{
    public interface IResponseProcessor
    {
        AdaptationRequestOutcome Process(IDictionary<string, object> headers, byte[] body);
    }
}
