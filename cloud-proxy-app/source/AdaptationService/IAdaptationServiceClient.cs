using System;
using System.Threading;

namespace Glasswall.IcapServer.CloudProxyApp.AdaptationService
{
    public interface IAdaptationServiceClient<IResponseProcessor> : IDisposable
    {
        void Connect();
        AdaptationRequestOutcome AdaptationRequest(Guid fileId, string originalStoreFilePath, string rebuiltStoreFilePath, CancellationToken processingCancellationToken);
    }
}
