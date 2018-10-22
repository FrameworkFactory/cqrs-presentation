using System;

namespace FWF.Security
{
    public interface ISecuritySessionFactory
    {

        ISecuritySession Create(
            ISecurityContext securityContext,
            string accessToken,
            string identityToken,
            string refreshToken,
            DateTime expiration
            );

    }
}

