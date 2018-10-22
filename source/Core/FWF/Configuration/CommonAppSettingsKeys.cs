
using System;

namespace FWF.Configuration
{
    public static class CommonAppSettingsKeys
    {

        public static readonly ReadOnlySetting EnvironmentNameSetting = new ReadOnlySetting("Environment.Name", "DEBUG");

        public static readonly ReadOnlySetting ClientIdSetting = new ReadOnlySetting("Client.Id", Guid.Empty.ToString());
        public static readonly ReadOnlySetting ClientNameSetting = new ReadOnlySetting("Client.Name", "My App");

        public static readonly ReadOnlySetting HttpHostPortSetting = new ReadOnlySetting("HttpHost.Port", "8080");

    }
}

