using System;

namespace FWF.Configuration
{
    public interface IAppSettings : IRunnable
    {

        string Get(string key);

        string Get(string key, string defaultValue);

        string Get(string key, string defaultValue, IFormatProvider formatProvider);

        string Get(ReadOnlySetting readOnlySetting);


        void AddOrUpdate(string settingName, string settingValue);


        string EnvironmentName { get; }
        Guid ClientId { get; }
        string ClientName { get; }
        int HttpHostPort { get; }

        bool IsDebugEnvironment { get; }

    }
}

