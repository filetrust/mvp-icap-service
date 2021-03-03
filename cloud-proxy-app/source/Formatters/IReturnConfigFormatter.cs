using Glasswall.IcapServer.CloudProxyApp.AdaptationService;

namespace Glasswall.IcapServer.CloudProxyApp.Formatters
{
    public interface IReturnConfigFormatter
    {
        string Write(AdaptationRequestOutcome outcome);
    }
}
