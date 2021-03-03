using Glasswall.IcapServer.CloudProxyApp.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Glasswall.IcapServer.CloudProxyApp.AdaptationService
{
    public class RabbitMqClient<TResponseProcessor> : IAdaptationServiceClient<TResponseProcessor> where TResponseProcessor : IResponseProcessor
    {
        private readonly IConnectionFactory connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private EventingBasicConsumer _consumer;
        private bool disposedValue;
        private readonly BlockingCollection<AdaptationRequestOutcome> _respQueue = new BlockingCollection<AdaptationRequestOutcome>();
        private readonly IResponseProcessor _responseProcessor;
        private readonly IQueueConfiguration _queueConfiguration;
        private readonly ILogger<RabbitMqClient<TResponseProcessor>> _logger;

        public RabbitMqClient(IResponseProcessor responseProcessor, IQueueConfiguration queueConfiguration, ILogger<RabbitMqClient<TResponseProcessor>> logger)
        {
            _responseProcessor = responseProcessor ?? throw new ArgumentNullException(nameof(responseProcessor));
            _queueConfiguration = queueConfiguration ?? throw new ArgumentNullException(nameof(queueConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            CheckCredentials(queueConfiguration);

            _logger.LogInformation($"Setting up queue connection '{queueConfiguration.MBHostName}:{queueConfiguration.MBPort}'");
            connectionFactory = new ConnectionFactory()
            {
                HostName = queueConfiguration.MBHostName,
                Port = queueConfiguration.MBPort,
                UserName = queueConfiguration.MBUsername,
                Password = queueConfiguration.MBPassword
            };
        }

        private void CheckCredentials(IQueueConfiguration queueConfiguration)
        {
            if (string.IsNullOrEmpty(queueConfiguration.MBUsername))
            {
                queueConfiguration.MBUsername = ConnectionFactory.DefaultUser;
                _logger.LogInformation("No RabbitMQ Username provided, using default");
            }
            if (string.IsNullOrEmpty(queueConfiguration.MBPassword))
            {
                queueConfiguration.MBPassword = ConnectionFactory.DefaultPass;
                _logger.LogInformation("No RabbitMQ Password provided, using default");
            }
        }

        public void Connect()
        {
            if (_connection != null || _channel != null || _consumer != null)
                throw new AdaptationServiceClientException("'Connect' should only be called once.");

            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _consumer = new EventingBasicConsumer(_channel);

            _consumer.Received += (model, ea) =>
            {
                try
                {
                    _logger.LogInformation($"Received message: Exchange Name: '{ea.Exchange}', Routing Key: '{ea.RoutingKey}'");
                    var headers = ea.BasicProperties.Headers;
                    var body = ea.Body.ToArray();

                    _respQueue.Add(_responseProcessor.Process(headers, body));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error Processing 'input'");
                    _respQueue.Add(AdaptationRequestOutcome.CreateError());
                }
            };
        }

        public AdaptationRequestOutcome AdaptationRequest(Guid fileId, string originalStoreFilePath, string rebuiltStoreFilePath, CancellationToken processingCancellationToken)
        {
            if (_connection == null || _channel == null || _consumer == null)
                throw new AdaptationServiceClientException("'Connect' should be called before 'AdaptationRequest'.");

            var queueDeclare = _channel.QueueDeclare(queue: _queueConfiguration.RequestQueueName,
                                                          durable: false,
                                                          exclusive: false,
                                                          autoDelete: false,
                                                          arguments: null);
            _logger.LogInformation($"Send Request Queue '{queueDeclare.QueueName}' Declared : MessageCount = {queueDeclare.MessageCount},  ConsumerCount = {queueDeclare.ConsumerCount}");

            IDictionary<string, object> headerMap = new Dictionary<string, object>
                    {
                        { "file-id", fileId.ToString() },
                        { "request-mode", "respmod" },
                        { "source-file-location", originalStoreFilePath},
                        { "rebuilt-file-location", rebuiltStoreFilePath},
                        { "generate-report", "true"}
                    };

            var messageProperties = _channel.CreateBasicProperties();
            messageProperties.Headers = headerMap;
            messageProperties.ReplyTo = _queueConfiguration.OutcomeQueueName;

            _logger.LogInformation($"Sending {_queueConfiguration.RequestMessageName} for {fileId}");

            _channel.BasicConsume(_consumer, _queueConfiguration.OutcomeQueueName, autoAck: true);

            _channel.BasicPublish(exchange: _queueConfiguration.ExchangeName,
                                 routingKey: _queueConfiguration.RequestMessageName,
                                 basicProperties: messageProperties);

            return _respQueue.Take(processingCancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _channel?.Dispose();
                    _connection?.Dispose();
                    _respQueue?.Dispose();
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
