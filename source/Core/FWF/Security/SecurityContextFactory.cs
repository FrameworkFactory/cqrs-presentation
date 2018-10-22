using FWF.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace FWF.Security
{
    internal class SecurityContextFactory : ISecurityContextFactory 
    {
        private class PrivateSecurityContext : ISecurityContext
        {
            public PrivateSecurityContext()
            {
                this.Roles = new List<ISecurityContextRole>();
                this.Entitlements = new List<Guid>();
            }

            public string AccessToken { get; set; }
            public Guid ClientId { get; set; }
            public Guid ClientInstanceId { get; set; }
            public Guid SessionId { get; set; }
            public Guid TenantId { get; set; }
            public string TenantName { get; set; }
            public Guid UserId { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Locale { get; set; }

            public IEnumerable<ISecurityContextRole> Roles { get; set; }
            public IEnumerable<Guid> Entitlements { get; set; }
        }

        private class PrivateSecurityContextRole : ISecurityContextRole
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public byte[] HierarchyMask { get; set; }
        }


        private readonly ILog _log;

        public SecurityContextFactory(
            ILogFactory logFactory)
        {
            _log = logFactory.CreateForType(this);
        }


        public ISecurityContext Create(ClaimsIdentity claimsIdentity)
        {
            if (claimsIdentity.IsNull())
            {
                return null;
            }

            var securityContext = new PrivateSecurityContext();

            return securityContext;
        }

    }
}



