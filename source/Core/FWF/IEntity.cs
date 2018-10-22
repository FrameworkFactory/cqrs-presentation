using System;

namespace FWF
{
    public interface IEntity
    {
        Guid Id
        {
            get; set;
        }

        string Name
        {
            get; set;
        }

        bool IsDeleted
        {
            get; set;
        }


        DateTime Created
        {
            get; set;
        }

        Guid CreatedBy
        {
            get; set;
        }

        DateTime LastModified
        {
            get; set;
        }

        Guid LastModifiedBy
        {
            get; set;
        }

        byte[] Version
        {
            get; set;
        }

    }
}



