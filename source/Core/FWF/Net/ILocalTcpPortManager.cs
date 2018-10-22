using System;

namespace FWF.Net
{
    public interface ILocalTcpPortManager
    {
        PortReservation GetNextAvailablePort();

    }
}

