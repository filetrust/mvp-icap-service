using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Glasswall.IcapServer.CloudProxyApp.AdaptationService
{
    public class AdaptationOutcomeProcessor : IResponseProcessor
    {
        private readonly ILogger<AdaptationOutcomeProcessor> _logger;
        private readonly IHeaderFilter _headerFilter;
        static readonly Dictionary<AdaptationOutcome, ReturnOutcome> OutcomeMap = new Dictionary<AdaptationOutcome, ReturnOutcome>
        {
            { AdaptationOutcome.Unmodified, ReturnOutcome.GW_UNPROCESSED},
            { AdaptationOutcome.Replace, ReturnOutcome.GW_REBUILT},
            { AdaptationOutcome.Failed, ReturnOutcome.GW_FAILED }
        };

        public AdaptationOutcomeProcessor(ILogger<AdaptationOutcomeProcessor> logger, IHeaderFilter headerFilter)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _headerFilter = headerFilter ?? throw new ArgumentNullException(nameof(headerFilter));
        }

        public ReturnOutcome Process(IDictionary<string, object> headers, byte[] body)
        {
            try
            {
                Guid fileId = Guid.Empty;
                if (!headers.ContainsKey("file-id"))
                    throw NewAdaptationServiceException("Missing File Id");

                var fileIdString = Encoding.UTF8.GetString((byte[])headers["file-id"]);
                if (fileIdString == null || !Guid.TryParse(fileIdString, out fileId))
                {
                    _logger.LogError($"Error in FileID: {fileIdString ?? "-"}");
                    return ReturnOutcome.GW_ERROR;
                }

                if (!headers.ContainsKey("file-outcome"))
                    throw NewAdaptationServiceException($"Missing outcome for File Id {fileId}");

                var outcomeHeaders = CheckForOutcomeHeaders(fileId, headers);

                var outcomeString = Encoding.UTF8.GetString((byte[])headers["file-outcome"]);
                AdaptationOutcome outcome = (AdaptationOutcome)Enum.Parse(typeof(AdaptationOutcome), outcomeString, ignoreCase: true);
                if (!OutcomeMap.ContainsKey(outcome))
                {
                    _logger.LogError($"Returning outcome unmapped: {outcomeString} for File Id {fileId}");
                    return ReturnOutcome.GW_ERROR;
                }
                return OutcomeMap[outcome];
            }
            catch (ArgumentException aex)
            {
                _logger.LogError($"Unrecognised enumeration processing adaptation outcome {aex.Message}");
                return ReturnOutcome.GW_ERROR;
            }
            catch (JsonReaderException jre)
            {
                _logger.LogError($"Poorly formated adaptation outcome : {jre.Message}");
                return ReturnOutcome.GW_ERROR;
            }
            catch (AdaptationServiceClientException asce)
            {
                _logger.LogError($"Poorly formated adaptation outcome : {asce.Message}");
                return ReturnOutcome.GW_ERROR;
            }
        }

        private IDictionary<string, string> CheckForOutcomeHeaders(Guid fileId, IDictionary<string, object> headers)
        {
            var result = _headerFilter.Extract(headers);
            _logger.LogInformation($"File Id {fileId}: {result.Count} outcome headers found");
            return result;
        }

        private AdaptationServiceClientException NewAdaptationServiceException(string message)
        {
            return new AdaptationServiceClientException(message);
        }
    }
}
