using System;

namespace FWF.Security
{
    internal class SecuritySessionFactory : ISecuritySessionFactory
    {

        private class PrivateSecuritySession : ISecuritySession
        {


            public string AccessToken { get; set; }
            public string IdentityToken { get; set; }
            public string RefreshToken { get; set; }

            public DateTime Expires { get; set; }

            public ISecurityContext SecurityContext { get; set; }
        }


        public ISecuritySession Create(
            ISecurityContext securityContext,
            string accessToken,
            string identityToken,
            string refreshToken,
            DateTime expiration
            )
        {
            // TODO: Validate that all data is correct

            var securitySession = new PrivateSecuritySession
            {
                AccessToken = accessToken,
                IdentityToken = identityToken,
                RefreshToken = refreshToken,
                Expires = expiration,
                SecurityContext = securityContext
            };

            return securitySession;
        }


    }
}

