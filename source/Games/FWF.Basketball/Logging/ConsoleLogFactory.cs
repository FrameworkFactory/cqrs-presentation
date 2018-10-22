using FWF.Logging;
using System;
using System.Globalization;

namespace FWF.Basketball.Logging
{
    internal class ConsoleLogFactory : Startable, ILogFactory
    {

        private const string DateTimeConsoleFormat = "HH:mm:ss.ffff";

        private readonly ConsoleLog _log;

        private class ConsoleLog : ILog
        {
            
            private static volatile object _lockObject = new object();

            private ConsoleLogFactory _logFactory;

            public ConsoleLog(ConsoleLogFactory logFactory)
            {
                _logFactory = logFactory;
            }

            public string Name
            {
                get { return "Console"; }
            }

            public bool IsLevelEnabled(LogLevel level)
            {
                return level >= LogLevel.Debug;
            }

            public void Log(LogPayload logPayload)
            {
                Write(logPayload);
            }

            public void Log(LogLevel level, string message)
            {
                Write(
                    new LogPayload
                    {
                        LogLevel = level,
                        Message = message
                    }
                    );
            }

            public void LogFormat(LogLevel level, string format, params object[] args)
            {
                Write(
                    new LogPayload
                    {
                        LogLevel = level,
                        Message = string.Format(format, args)
                    }
                    );
            }

            public void LogException(LogLevel level, Exception exception, string message)
            {
                Write(
                    new LogPayload
                    {
                        LogLevel = level,
                        Message = string.Concat(
                            message, 
                            Environment.NewLine, 
                            exception.RenderDetailString()
                            )
                    }
                    );
            }

            public void LogExceptionFormat(LogLevel level, Exception exception, string format, params object[] args)
            {
                Write(
                    new LogPayload
                    {
                        LogLevel = level,
                        Message = string.Concat(
                            string.Format(format, args),
                            Environment.NewLine,
                            exception.RenderDetailString()
                            )
                    }
                    );
            }

            public void Write(LogPayload logPayload)
            {
                lock (_lockObject)
                {
                    _logFactory.WriteConsoleDashboard(logPayload);
                }
            }

        }

        public ConsoleLogFactory()
        {
            _log = new ConsoleLog(this);
        }

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        public ILog CreateForType(object instance)
        {
            if (!IsRunning)
            {
                throw new Exception("Component not started - ILogFactory");
            }

            if (ReferenceEquals(instance, null))
            {
                throw new ArgumentNullException("instance");
            }

            return _log;
        }

        public void WriteConsoleDashboard(LogPayload logPayload)
        {
            // TODO: Determine best way to write this...
            Write(logPayload);
        }


        private void Write(LogPayload logPayload)
        {
            ConsoleColor consoleColor = Console.ForegroundColor;

            var stringBuilder = StringBuilderPool.Acquire();
            stringBuilder.Append(DateTime.UtcNow.ToString(DateTimeConsoleFormat, CultureInfo.CurrentCulture));
            stringBuilder.Append(" ");
            stringBuilder.Append(("[" + logPayload.LogLevel.Name + "]").PadRight(8));
            stringBuilder.Append(logPayload.Name);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Out.WriteLine(stringBuilder.ToString());

            StringBuilderPool.Release(stringBuilder);

            Console.ForegroundColor = FetchColorByLevel(logPayload.LogLevel);

            Console.Out.WriteLine(logPayload.Message);
            
            if (logPayload.Callstack != null)
            {
                Console.Out.WriteLine(logPayload.Callstack);
            }

            Console.ForegroundColor = consoleColor;

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
