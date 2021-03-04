using System.Collections.Generic;

namespace Glasswall.IcapServer.CloudProxyApp
{
    public static class CommandLineSwitchMapping
    {
        public const string InputConfigurationKey = "InputFilepath";
        public const string OutputConfigurationKey = "OutputFilepath";
        public const string FileIdConfigurationKey = "FileId";
        public const string ReturnConfigurationKey = "ReturnConfigFilepath";

        public static IDictionary<string, string> Mapping { get; } = new Dictionary<string, string>
        {
                { "-i", InputConfigurationKey },
                { "-o", OutputConfigurationKey },
                { "-f", FileIdConfigurationKey },
                { "-c", ReturnConfigurationKey }
        };
    }
}
