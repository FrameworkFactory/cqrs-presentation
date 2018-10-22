using FWF.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace FWF.Test.Logging
{
    [DebuggerStepThrough]
    public class UnitTestLog : ILog
    {
        private readonly string _logName;
        private readonly LogLevel _logLevel;

        private const string DateTimeConsoleFormat = "HH:mm:ss.ffff";

        private static volatile object _lockObject = new object();

        public UnitTestLog(string logName)
        {
            _logName = logName;

            var optionalLogLevelSetting = ConfigurationManager.AppSettings.Get("UnitTestLog.LogLevel");

            if (optionalLogLevelSetting.IsPresent())
            {
                int optionalLogLevelSettingValue;

                if (int.TryParse(optionalLogLevelSetting, out optionalLogLevelSettingValue))
                {
                    _logLevel = optionalLogLevelSettingValue;
                }
            }

            if (ReferenceEquals(_logLevel, null))
            {
                _logLevel = LogLevel.Debug;
            }

        }

        public string Name
        {
            get { return _logName; }
        }

        public bool IsLevelEnabled(LogLevel level)
        {
            return _logLevel <= level;
        }

        public void Log(LogPayload logPayload)
        {
            WriteInternal(logPayload.LogLevel, logPayload.Message, null);
        }

        public void Log(LogLevel level, string message)
        {
            WriteInternal(level, message, null);
        }

        public void LogFormat(LogLevel level, string format, params object[] args)
        {
            WriteInternal(level, string.Format(format, args), null);
        }

        public void LogException(LogLevel level, Exception exception, string message)
        {
            WriteInternal(level, message, exception);
        }

        public void LogExceptionFormat(LogLevel level, Exception exception, string format, params object[] args)
        {
            WriteInternal(level, string.Format(format, args), exception);
        }

        public void LogProperties(LogLevel level, IDictionary<string, string> propertiesDictionary)
        {
            // Do nothing
        }

        public void LogProperties(LogLevel level, IDictionary<string, string> propertiesDictionary, string message)
        {
            // Do nothing
        }

        public void LogProperties(LogLevel level, IDictionary<string, string> propertiesDictionary, string message, params object[] args)
        {
            // Do nothing
        }

        private void WriteInternal(LogLevel level, string message, Exception exception)
        {
            if (_logLevel > level)
            {
                return;
            }

            var logPayload = new LogPayload
            {
                Name = this.Name,
                LogLevel = level,
                Message = message
            };

            lock (_lockObject)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append(DateTime.UtcNow.ToString(DateTimeConsoleFormat, CultureInfo.CurrentCulture));
                stringBuilder.Append(" ");
                stringBuilder.Append(("[" + logPayload.LogLevel.Name + "]").PadRight(8));
                stringBuilder.Append(logPayload.Name);

                //Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(stringBuilder.ToString());

                //Console.ForegroundColor = FetchColorByLevel(logPayload.LogLevel);
                Console.WriteLine(logPayload.Message.ToString());

                //Console.ResetColor();
            }
        }

        private static ConsoleColor FetchColorByLevel(LogLevel logLevel)
        {
            if (logLevel >= LogLevel.Error)
            {
                return ConsoleColor.Red;
            }

            if (logLevel >= LogLevel.Warn)
            {
                return ConsoleColor.DarkYellow;
            }

            return ConsoleColor.White;
        }
    }
}



