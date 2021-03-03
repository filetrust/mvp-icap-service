using System.Collections.Generic;

namespace Glasswall.IcapServer.CloudProxyApp.AdaptationService
{
    public class AdaptationRequestOutcome
    {
        static public AdaptationRequestOutcome CreateError()
        {
            return new AdaptationRequestOutcome
            {
                Outcome = ReturnOutcome.GW_ERROR,
                OutcomeHeaders = new Dictionary<string, string>()
            };
        }

        public ReturnOutcome Outcome    { get; set; }

        public IDictionary<string, string> OutcomeHeaders { get; set; }
    }
}
