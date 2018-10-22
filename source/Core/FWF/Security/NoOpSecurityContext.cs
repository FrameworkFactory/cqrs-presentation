using System;
using System.Collections.Generic;

namespace FWF.Security
{
    // FIX: This shouldn't be public.  Using this as a temp object until token auth/authz is implemented...
    public class NoOpSecurityContext : ISecurityContext
    {

        private class NoOpSecurityContextRole : ISecurityContextRole
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public byte[] HierarchyMask { get; set; }
        }

        
        public NoOpSecurityContext()
        {
            this.Roles = new List<ISecurityContextRole>();
            this.Entitlements = new List<Guid>();
        }

        public string AccessToken { get; set; }

        public Guid TenantId { get; set; }

        public string TenantName { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Locale { get; set; }

        public Guid ClientId { get; set; }

        public Guid ClientInstanceId { get; set; }

        public Guid SessionId { get; set; }

        public IEnumerable<ISecurityContextRole> Roles { get; set; }

        public IEnumerable<Guid> Entitlements { get; set; }
    }
}

