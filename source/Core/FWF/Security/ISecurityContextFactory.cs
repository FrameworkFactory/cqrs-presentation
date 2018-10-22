using System.Security.Claims;

namespace FWF.Security
{
    public interface ISecurityContextFactory
    {

        ISecurityContext Create(ClaimsIdentity claimsIdentity);

    }
}

