namespace Glasswall.IcapServer.CloudProxyApp.Configuration
{
    public static class AppConfigurationExtensions
    {
        public static bool ReturnConfigFilePathSpecified(this IAppConfiguration appConfiguration)
        {
            return !string.IsNullOrEmpty(appConfiguration.ReturnConfigFilepath);
        }
    }
}
