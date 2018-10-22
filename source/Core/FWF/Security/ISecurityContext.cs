using System;
using System.Collections.Generic;

namespace FWF.Security
{
    public interface ISecurityContext
    {
        string AccessToken { get; set; }

        Guid TenantId { get; }

        string TenantName { get; }

        Guid UserId { get; }

        string UserName { get; }

        string Email { get; }

        string Locale { get; }

        Guid ClientId { get; }

        Guid ClientInstanceId { get; }

        Guid SessionId { get; }

        IEnumerable<ISecurityContextRole> Roles { get; }

        IEnumerable<Guid> Entitlements { get; }

    }
}



