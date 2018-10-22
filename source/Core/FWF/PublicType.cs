using System;

namespace FWF
{
    public class PublicType
    {

        private string _name;
        private AppVersion _version;
        private Type _type;

        public static PublicType FromType(Type type)
        {
            if (ReferenceEquals(type, null))
            {
                throw new ArgumentNullException("type");
            }
            if (type.IsInterface)
            {
                throw new InvalidOperationException("Cannot create public interface type");
            }
            if (type.IsAbstract)
            {
                throw new InvalidOperationException("Cannot create public abstract type");
            }

            // NOTE: Generic types are flexible and would be nearly impossible to deserialize
            if (type.IsGenericType)
            {
                throw new InvalidOperationException("Cannot create public generic type");
            }

            var publicType = new PublicType
            {
                _name = type.AssemblyQualifiedName,
                _version = AppVersion.Parse(type.Assembly.GetName().Version.ToString()),
                _type = type,
            };

            return publicType;
        }

        public static PublicType FromName(string publicTypeName, AppVersion typeVersion)
        {
            var knownType = Type.GetType(publicTypeName, false, true);

            if (knownType.IsNull())
            {
                throw new InvalidOperationException("Unable to determine .NET Type from: " + publicTypeName);
            }

            return FromType(knownType);
        }

        public string Name
        {
            get { return _name; }
        }

        public AppVersion Version
        {
            get { return _version; }
        }

        public Type Type
        {
            get { return _type; }
        }
    }
}
