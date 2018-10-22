using FWF.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace FWF.Configuration
{
    internal class InMemoryAppSettings : Startable, IAppSettings
    {

        private readonly IDictionary<string, object> _items = new SortedDictionary<string, object>();
        private volatile object _lockObject = new object();

        public InMemoryAppSettings()
        {
            // NOTE: No logger here.  I expect some settings to exist with a logging implementation and don't
            // want a circular reference.
        }

        protected override void OnStart()
        {
            var isUnitTest = AppDomain.CurrentDomain.FriendlyName.EqualsIgnoreCase("testhost");

            var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
            var exe = System.Reflection.Assembly.GetExecutingAssembly();
            var caller = System.Reflection.Assembly.GetCallingAssembly();

            // NOTE: The .NET Core unit test adapters do not work with the 
            // System.Configuration.ConfigurationManager shim.  Need to detect when
            // running as a unit test to determine how to load the config file
            if (isUnitTest)
            {
                var configFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.config");

                if (configFiles.Any())
                {
                    var configFile = configFiles.First();

                    var configFileWithoutExtension = Path.GetFileNameWithoutExtension(configFile);

                    var config = ConfigurationManager.OpenExeConfiguration(configFileWithoutExtension);

                    var appSettings = config.AppSettings.Settings;
                    
                    foreach (KeyValueConfigurationElement setting in appSettings)
                    {
                        AddOrUpdate(setting.Key, setting.Value);
                    }
                }
            }
            else
            {
                // Add each app setting and connection string to the in-memory config

                foreach (string key in ConfigurationManager.AppSettings.Keys)
                {
                    AddOrUpdate(key, ConfigurationManager.AppSettings[key]);
                }
            }

        }

        protected override void OnStop()
        {
            _items.Clear();
        }


        public string Get(string key)
        {
            return Get(key, string.Empty);
        }

        public string Get(string key, string defaultValue)
        {
            return Get(key, defaultValue, CultureInfo.InvariantCulture);
        }

        public string Get(string key, string defaultValue, IFormatProvider formatProvider)
        {
            if (!_items.ContainsKey(key))
            {
                AddOrUpdate(key, defaultValue);

                return defaultValue;
            }

            var appSettingValue = _items[key];

            var appSettingValueString = appSettingValue as string;

            if (appSettingValueString.IsMissing())
            {
                return defaultValue;
            }

            return appSettingValueString;
        }

        public string Get(ReadOnlySetting setting)
        {
            return Get(setting.Name, setting.Default, CultureInfo.InvariantCulture);
        }

        public void AddOrUpdate(string settingName, string settingValue)
        {
            lock (_lockObject)
            {
                _items[settingName] = settingValue;
            }
        }
        


        // NOTE: Provide defaults

        public string EnvironmentName { get { return Get(CommonAppSettingsKeys.EnvironmentNameSetting); } }
        public Guid ClientId { get { return Guid.Parse(Get(CommonAppSettingsKeys.ClientIdSetting)); } }
        public string ClientName { get { return Get(CommonAppSettingsKeys.ClientNameSetting); } }
        public int HttpHostPort { get { return int.Parse(Get(CommonAppSettingsKeys.HttpHostPortSetting)); } }


        public bool IsDebugEnvironment
        {
            get { return EnvironmentName.EqualsIgnoreCase("DEBUG"); }
        }



    }
}

