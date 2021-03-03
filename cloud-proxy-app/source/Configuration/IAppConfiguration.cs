namespace Glasswall.IcapServer.CloudProxyApp.Configuration
{
    public interface IAppConfiguration
    {
        string FileId { get; }
        string InputFilepath { get; }
        string OutputFilepath { get; }
        string ReturnConfigFilepath { get; }
    }
}
