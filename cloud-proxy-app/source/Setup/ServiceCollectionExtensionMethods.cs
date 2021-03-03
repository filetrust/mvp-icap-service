using Glasswall.IcapServer.CloudProxyApp.AdaptationService;
using Glasswall.IcapServer.CloudProxyApp.ConfigLoaders;
using Glasswall.IcapServer.CloudProxyApp.Configuration;
using Glasswall.IcapServer.CloudProxyApp.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Glasswall.IcapServer.CloudProxyApp.Setup
{
    public static class ServiceCollectionExtensionMethods
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddLogging(configure => configure.AddConsole());

            serviceCollection.AddTransient<NativeProxyApplication>();

            var appConfig = new CloudProxyApplicationConfiguration();
            configuration.Bind(appConfig);
            serviceCollection.AddSingleton<IAppConfiguration>(appConfig);

            var queueConfig = RabbitMqDefaultConfigLoader.SetDefaults(new RabbitMqQueueConfiguration());
            configuration.Bind(queueConfig);
            serviceCollection.AddSingleton<IQueueConfiguration>(queueConfig);

            var storeConfig = AdaptationStoreConfigLoader.SetDefaults(new AdaptationStoreConfiguration());
            configuration.Bind(storeConfig);
            serviceCollection.AddSingleton<IStoreConfiguration>(storeConfig);

            var processingConfig = IcapProcessingConfigLoader.SetDefaults(new IcapProcessingConfiguration());
            configuration.Bind(processingConfig);
            serviceCollection.AddSingleton<IProcessingConfiguration>(processingConfig);

            serviceCollection.AddTransient(typeof(IAdaptationServiceClient<>), typeof(RabbitMqClient<>));
            serviceCollection.AddTransient<IResponseProcessor, AdaptationOutcomeProcessor>();
            serviceCollection.AddTransient<IHeaderFilter, OutcomeHeaderFilter>();

            serviceCollection.AddTransient<IReturnConfigFormatter, JsonReturnConfigFormatter>();

            return serviceCollection;
        }
    }
}
