using Glasswall.IcapServer.CloudProxyApp.AdaptationService;
using Glasswall.IcapServer.CloudProxyApp.Configuration;
using Glasswall.IcapServer.CloudProxyApp.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Glasswall.IcapServer.CloudProxyApp.Tests.Setup
{
    class ServiceCollectionExtentionMethodsTests
    {
        private ServiceCollection _serviceCollection;
        private ConfigurationBuilder _configurationBuilder;

        [SetUp]
        public void ServiceCollectionExtentionMethodsTestsSetup()
        {
            _serviceCollection = new ServiceCollection();
            _configurationBuilder = new ConfigurationBuilder();
        }

        [Test]
        public void NativeProxyApplication_is_added_as_transient()
        {
            // Arrange
            IConfiguration configuration = _configurationBuilder.Build();

            // Act
            var serviceProvider = _serviceCollection.ConfigureServices(configuration).BuildServiceProvider(true);
            var nativeProxyApplication = serviceProvider.GetService<NativeProxyApplication>();
            var secondNativeProxyApplication = serviceProvider.GetService<NativeProxyApplication>();

            // Assert
            Assert.That(nativeProxyApplication, Is.Not.Null, "expected the object to be available");
            Assert.AreNotSame(nativeProxyApplication, secondNativeProxyApplication, "expected the same object to be provided");
        }

        [Test]
        public void ApplicationConfiguration_is_added_as_singleton()
        {
            // Arrange
            IConfiguration configuration = _configurationBuilder.Build();

            // Act
            var serviceProvider = _serviceCollection.ConfigureServices(configuration).BuildServiceProvider(true);
            var appConfiguration = serviceProvider.GetService<IAppConfiguration>();
            var secondAppConfiguration = serviceProvider.GetService<IAppConfiguration>();

            // Assert
            Assert.That(appConfiguration, Is.Not.Null, "expected the object to be available");
            Assert.AreSame(appConfiguration, secondAppConfiguration, "expected the same object to be provided");
        }

        [Test]
        public void Supplied_ApplicationConfiguration_is_bound()
        {
            // Arrange
            const string TestInputFilepath = @"c:\testinput\file.pdf";
            const string TestOutputFilepath = @"c:\testoutput\file.pdf";
            var testConfiguration = new Dictionary<string, string>()
            {
                [nameof(IAppConfiguration.InputFilepath)] = TestInputFilepath,
                [nameof(IAppConfiguration.OutputFilepath)] = TestOutputFilepath
            };

            IConfiguration configuration = _configurationBuilder
                                                    .AddInMemoryCollection(testConfiguration)
                                                    .Build();

            // Act
            var serviceProvider = _serviceCollection.ConfigureServices(configuration).BuildServiceProvider(true);
            var appConfiguration = serviceProvider.GetService<IAppConfiguration>();

            // Assert
            Assert.That(appConfiguration.InputFilepath, Is.EqualTo(TestInputFilepath), "expected the input filepath to be bound");
            Assert.That(appConfiguration.OutputFilepath, Is.EqualTo(TestOutputFilepath), "expected the output filepath to be bound");
        }

        [Test]
        public void QueueConfiguration_is_added_as_singleton()
        {
            // Arrange
            IConfiguration configuration = _configurationBuilder.Build();

            // Act
            var serviceProvider = _serviceCollection.ConfigureServices(configuration).BuildServiceProvider(true);
            var queueConfiguration = serviceProvider.GetService<IQueueConfiguration>();
            var secondQueueConfiguration = serviceProvider.GetService<IQueueConfiguration>();

            // Assert
            Assert.That(queueConfiguration, Is.Not.Null, "expected the object to be available");
            Assert.AreSame(queueConfiguration, secondQueueConfiguration, "expected the same object to be provided");
        }

        [Test]
        public void OutcomeHeaderFilter_is_added_as_transitory()
        {
            // Arrange
            IConfiguration configuration = _configurationBuilder.Build();

            // Act
            var serviceProvider = _serviceCollection.ConfigureServices(configuration).BuildServiceProvider(true);
            var outcomeHeaderFilter = serviceProvider.GetService<IHeaderFilter>();
            var secondOutcomeHeaderFilter = serviceProvider.GetService<IHeaderFilter>();

            // Assert
            Assert.That(outcomeHeaderFilter, Is.Not.Null, "expected the object to be available");
            Assert.AreNotSame(outcomeHeaderFilter, secondOutcomeHeaderFilter, "don't expect the same object to be provided");
        }

        [Test]
        public void Supplied_QueueConfiguration_is_bound()
        {
            // Arrange
            const string TestMBHostName = "Test MBHostName";
            const string TestMBPort = "1324";
            const string TestExchangeName = "Test ExchangeName";
            const string TestRequestQueueName = "Test RequestQueueName";
            const string TestOutcomeQueueName = "Test OutcomeQueueName";
            const string TestRequestMessageName = "Test RequestMessageName";

            var testConfiguration = new Dictionary<string, string>()
            {
                [nameof(IQueueConfiguration.MBHostName)] = TestMBHostName,
                [nameof(IQueueConfiguration.MBPort)] = TestMBPort,
                [nameof(IQueueConfiguration.ExchangeName )] = TestExchangeName,
                [nameof(IQueueConfiguration.RequestQueueName)] = TestRequestQueueName,
                [nameof(IQueueConfiguration.OutcomeQueueName)] = TestOutcomeQueueName,
                [nameof(IQueueConfiguration.RequestMessageName)] = TestRequestMessageName,                
            };

            IConfiguration configuration = _configurationBuilder
                                                    .AddInMemoryCollection(testConfiguration)
                                                    .Build();

            // Act
            var serviceProvider = _serviceCollection.ConfigureServices(configuration).BuildServiceProvider(true);
            var appConfiguration = serviceProvider.GetService<IQueueConfiguration>();

            // Assert
            Assert.That(appConfiguration.MBHostName, Is.EqualTo(TestMBHostName), "expected the hostname to be bound");
            Assert.That(appConfiguration.MBPort, Is.EqualTo(Convert.ToInt32(TestMBPort)), "expected the port to be bound");
            Assert.That(appConfiguration.ExchangeName, Is.EqualTo(TestExchangeName), "expected the exchange name to be bound");
            Assert.That(appConfiguration.RequestQueueName, Is.EqualTo(TestRequestQueueName), "expected the request queue name to be bound");
            Assert.That(appConfiguration.OutcomeQueueName, Is.EqualTo(TestOutcomeQueueName), "expected the outcome queue name to be bound");
            Assert.That(appConfiguration.RequestMessageName, Is.EqualTo(TestRequestMessageName), "expected the request message name to be bound");
        }
    }
}
