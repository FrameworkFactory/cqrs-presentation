using FWF.Security;
using System;

namespace FWF.Test.Security
{
    public class TestSecurityContextRole : ISecurityContextRole 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte[] HierarchyMask { get; set; }
    }
}


