using System.Threading;

namespace FWF.Net
{
    public class PortReservation : DisposableObject
    {
        private int _port;
        private Mutex _waitHandle;

        public PortReservation(int port, Mutex waitHandle)
        {
            _port = port;
            _waitHandle = waitHandle;
        }

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _waitHandle.ReleaseMutex();
                _waitHandle.Dispose();
            }
            base.Dispose(disposing);
        }

        public int Port
        {
            get { return _port; }
        }

    }

}

