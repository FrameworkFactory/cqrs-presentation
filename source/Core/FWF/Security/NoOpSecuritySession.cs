using System;

namespace FWF.Security
{
    public class NoOpSecuritySession : ISecuritySession
    {

        public NoOpSecuritySession()
        {
            this.SecurityContext = new NoOpSecurityContext();
        }

        public ISecurityContext SecurityContext
        {
            get; set;
        }

        public string AccessToken { get; set; }
        public DateTime Expires { get; set; }

    }
}

