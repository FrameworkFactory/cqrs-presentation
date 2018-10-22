using FWF.Security;
using System;

namespace FWF.Test.Security
{
    public class TestSecuritySession : ISecuritySession
    {
        public TestSecuritySession()
        {
            this.SecurityContext = new TestSecurityContext();
        }

        public string AccessToken { get; set; }
        public string IdentityToken { get; set; }
        public string RefreshToken { get; set; }

        public DateTime Expires { get; set; }

        public ISecurityContext SecurityContext { get; set; }

    }
}

