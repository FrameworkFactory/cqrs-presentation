using FWF.Configuration;
using FWF.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace FWF.Net
{
    internal class LocalTcpPortManager : ILocalTcpPortManager
    {

        private readonly List<PortRangeItem> _items = new List<PortRangeItem>();
        private volatile object _lockObject = new object();

        private readonly IAppSettings _appSettings;
        private readonly ILog _log;

        
        private class PortRangeItem
        {
            public string EnvironmentName
            {
                get; set;
            }

            public int PortMin
            {
                get; set;
            }

            public int PortMax
            {
                get; set;
            }
        }

        public LocalTcpPortManager(
            IAppSettings appSettings,
            ILogFactory logFactory
            )
        {
            _appSettings = appSettings;

            _log = logFactory.CreateForType(this);

            _items.Add(new PortRangeItem
            {
                EnvironmentName = "DEBUG",
                PortMin = 24000,
                PortMax = 24999,
            });
            _items.Add(new PortRangeItem
            {
                EnvironmentName = "RELEASE",
                PortMin = 52000,
                PortMax = 52999,
            });
        }


        public PortReservation GetNextAvailablePort()
        {
            var item = _items.FirstOrDefault(
                x => _appSettings.EnvironmentName.EqualsIgnoreCase(x.EnvironmentName)
                );

            if (item.IsNull())
            {
                throw new InvalidOperationException();
            }

            PortReservation portReservation = null;

            // Attempt to reserve across processes
            Mutex mutex = null;

            while (true)
            {
                mutex = new Mutex(false, "LocalTcpPortManager");

                var didSignal = mutex.WaitOne(TimeSpan.FromSeconds(9));

                if (didSignal)
                {
                    break;
                }

                mutex.ReleaseMutex();
                mutex.Dispose();

                _log.Debug("Waiting for a port reservation");
            }

            //
            var ipGP = IPGlobalProperties.GetIPGlobalProperties();
            var endpoints = ipGP.GetActiveTcpListeners();

            for (int i = item.PortMin; i < item.PortMax; i++)
            {
                var listener = endpoints.FirstOrDefault(x => x.Port == i);

                if (listener.IsNull())
                {
                    portReservation = new PortReservation(i, mutex);
                    break;
                }
            }

            if (portReservation.IsNull())
            {
                mutex.ReleaseMutex();
                mutex.Dispose();

                throw new InvalidOperationException("Unable to locate available port");
            }

            return portReservation;
        }

    }
}

