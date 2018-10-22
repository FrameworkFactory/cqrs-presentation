using System;

namespace FWF.Security
{
    public interface ISecuritySession
    {

        string AccessToken { get; }
        DateTime Expires { get; }

        ISecurityContext SecurityContext { get; }

    }
}

