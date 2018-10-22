using System;

namespace FWF
{
    public abstract class Entity<T> :
        IEntity,
        IEquatable<T>,
        IComparable<T>
        where T : class, IEntity
    {
        private Guid _id;
        private string _name = string.Empty;

        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public DateTime Created
        {
            get; set;
        }

        public Guid CreatedBy
        {
            get; set;
        }

        public DateTime LastModified
        {
            get; set;
        }

        public Guid LastModifiedBy
        {
            get; set;
        }

        public bool IsDeleted
        {
            get; set;
        }

        public byte[] Version
        {
            get; set;
        }

        #region Object Overrides

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format(
                "{0}:{1}",
                _id.ToString("B"),
                _name
                );
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            return Equals(obj as T);
        }

        #endregion

        #region IEquatable Members

        public bool Equals(T other)
        {
            return IsEqualKey(other);
        }

        #endregion

        #region IComparable Members

        public int CompareTo(T other)
        {
            if (ReferenceEquals(other, null))
            {
                return 1;
            }

            return string.CompareOrdinal(Name, other.Name);
        }

        #endregion

        public bool IsEqualKey(IEntity entity)
        {
            if (ReferenceEquals(entity, null))
            {
                return false;
            }

            return Id == entity.Id;
        }

        public bool IsEqualVersion(IEntity entity)
        {
            if (ReferenceEquals(entity, null))
            {
                return false;
            }

            return Version.IsEqualByte(entity.Version);
        }

    }
}

