using Glasswall.IcapServer.CloudProxyApp.AdaptationService;
using Glasswall.IcapServer.CloudProxyApp.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Glasswall.IcapServer.CloudProxyApp
{
    public class NativeProxyApplication : IDisposable
    {
        private readonly IAppConfiguration _appConfiguration;
        private readonly ILogger<NativeProxyApplication> _logger;
        private readonly CancellationTokenSource _processingCancellationTokenSource;
        private readonly TimeSpan _processingTimeoutDuration;
        private readonly string OriginalStorePath;
        private readonly string RebuiltStorePath;

        private readonly IAdaptationServiceClient<AdaptationOutcomeProcessor> _adaptationServiceClient;
        private bool disposedValue;

        public NativeProxyApplication(IAdaptationServiceClient<AdaptationOutcomeProcessor> adaptationServiceClient,
            IAppConfiguration appConfiguration, IStoreConfiguration storeConfiguration, IProcessingConfiguration processingConfiguration, ILogger<NativeProxyApplication> logger)
        {
            _adaptationServiceClient = adaptationServiceClient ?? throw new ArgumentNullException(nameof(adaptationServiceClient));
            _appConfiguration = appConfiguration ?? throw new ArgumentNullException(nameof(appConfiguration));
            if (storeConfiguration == null) throw new ArgumentNullException(nameof(storeConfiguration));
            if (processingConfiguration == null) throw new ArgumentNullException(nameof(processingConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _processingTimeoutDuration = processingConfiguration.ProcessingTimeoutDuration;
            _processingCancellationTokenSource = new CancellationTokenSource(_processingTimeoutDuration);

            OriginalStorePath = storeConfiguration.OriginalStorePath;
            RebuiltStorePath = storeConfiguration.RebuiltStorePath;
        }

        public Task<int> RunAsync()
        {
            string originalStoreFilePath = string.Empty;
            string rebuiltStoreFilePath = string.Empty;
            var fileId = GetFileId(_appConfiguration.FileId);
            try
            {
                var processingCancellationToken = _processingCancellationTokenSource.Token;

                _logger.LogInformation($"Using store locations '{OriginalStorePath}' and '{RebuiltStorePath}' for {fileId}");

                originalStoreFilePath = Path.Combine(OriginalStorePath, fileId.ToString());
                rebuiltStoreFilePath = Path.Combine(RebuiltStorePath, fileId.ToString());

                _logger.LogInformation($"FileId:{fileId}:Updating 'Original' store for {fileId}");
                File.Copy(_appConfiguration.InputFilepath, originalStoreFilePath, overwrite: true);

                _adaptationServiceClient.Connect();
                var requestOutcome = _adaptationServiceClient.AdaptationRequest(fileId, originalStoreFilePath, rebuiltStoreFilePath, processingCancellationToken);

                if (requestOutcome.Outcome == ReturnOutcome.GW_REBUILT || requestOutcome.Outcome == ReturnOutcome.GW_FAILED)
                {
                    _logger.LogInformation($"FileId:{fileId}:Copy from '{rebuiltStoreFilePath}' to {_appConfiguration.OutputFilepath}");
                    File.Copy(rebuiltStoreFilePath, _appConfiguration.OutputFilepath, overwrite: true);
                }

                if (_appConfiguration.ReturnConfigFilePathSpecified())
                {

                }

                ClearStores(fileId, originalStoreFilePath, rebuiltStoreFilePath);

                _logger.LogInformation($"Returning '{requestOutcome.Outcome}' Outcome for {fileId}");
                return Task.FromResult((int)requestOutcome.Outcome);
            }
            catch (OperationCanceledException oce)
            {
                _logger.LogError(oce, $"FileId:{fileId}:Error Processing Timeout exceeded {_processingTimeoutDuration.TotalSeconds}s");
                ClearStores(fileId, originalStoreFilePath, rebuiltStoreFilePath);
                return Task.FromResult((int)ReturnOutcome.GW_ERROR);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"FileId:{fileId}:Error Processing 'input'");
                ClearStores(fileId, originalStoreFilePath, rebuiltStoreFilePath);
                return Task.FromResult((int)ReturnOutcome.GW_ERROR);
            }
        }

        private Guid GetFileId(string fileId)
        {
            if (!Guid.TryParse(fileId, out Guid guidFileId))
            {
                guidFileId = Guid.NewGuid();
                _logger.LogInformation($"No valid FileId provided, substituting '{guidFileId}'");                
            }
            return guidFileId;
        }

        private void ClearStores(Guid fileId, string originalStoreFilePath, string rebuiltStoreFilePath)
        {
            try
            {
                _logger.LogInformation($"FileId:{fileId}:Clearing stores '{originalStoreFilePath}' and {rebuiltStoreFilePath}");
                if (!string.IsNullOrEmpty(originalStoreFilePath))
                    File.Delete(originalStoreFilePath);
                if (!string.IsNullOrEmpty(rebuiltStoreFilePath))
                    File.Delete(rebuiltStoreFilePath);
            }

            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Error whilst attempting to clear stores: {ex.Message}");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _adaptationServiceClient?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
