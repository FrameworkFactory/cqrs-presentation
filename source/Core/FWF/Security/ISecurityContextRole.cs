using System;

namespace FWF.Security
{
    public interface ISecurityContextRole
    {

        Guid Id { get; }

        string Name { get; }

        byte[] HierarchyMask { get; }

    }
}


