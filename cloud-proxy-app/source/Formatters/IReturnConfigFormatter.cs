using System;
using System.Collections.Generic;
using System.Text;

namespace Glasswall.IcapServer.CloudProxyApp.Formatters
{
    public interface IReturnConfigFormatter
    {
        string Write(ReturnOutcome outcome, string returnConfigFilepath);
    }
}
