using FWF.Security;
using System;
using System.Collections.Generic;

namespace FWF.Test.Security
{
    public class TestSecurityContext : ISecurityContext
    {

        public TestSecurityContext()
        {
            Roles = new List<ISecurityContextRole>();
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

        public TestSecurityContext Clone()
        {
            var returnItem = new TestSecurityContext();
            returnItem.TenantId = TenantId;
            returnItem.TenantName = TenantName;
            returnItem.UserId = UserId;
            returnItem.UserName = UserName;
            returnItem.Email = Email;
            returnItem.Locale = Locale;
            returnItem.ClientId = ClientId;
            returnItem.ClientInstanceId = ClientInstanceId;
            returnItem.SessionId = SessionId;
            var roles = new List<TestSecurityContextRole>();
            var entitlements = new List<Guid>();

            foreach (var role in Roles)
            {
                var clonedRole = new TestSecurityContextRole();
                clonedRole.Id = role.Id;
                clonedRole.Name = role.Name;
                clonedRole.HierarchyMask = role.HierarchyMask;
                roles.Add(clonedRole);
            }
            foreach (var entitlement in Entitlements)
            {
                entitlements.Add(entitlement);
            }
            returnItem.Roles = roles;
            returnItem.Entitlements = entitlements;
            return returnItem;
        }
    }
}


