namespace Glasswall.IcapServer.CloudProxyApp.Configuration
{
    public class CloudProxyApplicationConfiguration : IAppConfiguration
    {
        public string FileId { get; set; }

        public string InputFilepath { get; set; }

        public string OutputFilepath { get; set; }

        public string ReturnConfigFilepath { get; set; }
    }
}
