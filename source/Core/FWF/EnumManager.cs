using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace FWF
{
    public struct EnumItem
    {
        public object Id
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
    }

    public static class EnumManager
    {

        public static IEnumerable<EnumItem> ToList(Type enumType)
        {
            var listNames = Enum.GetNames(enumType);
            var listValues = Enum.GetValues(enumType);

            return listNames.Select(
                (t, i) =>
                new EnumItem
                {
                    Id = listValues.GetValue(i),
                    Name = t
                }
                );
        }

        public static T Parse<T>(object data, T defaultValue)
        {
            var obj = Parse(data, typeof(T));

            if (ReferenceEquals(obj, null))
            {
                return defaultValue;
            }

            return (T)obj;
        }

        public static object Parse(object data, Type type)
        {
            if (ReferenceEquals(data, null))
            {
                return null;
            }

            try
            {
                if (data is string)
                {
                    if (string.IsNullOrEmpty(data as string))
                    {
                        return null;
                    }

                    // Parse the enum string, which may contain multiple values delimited by a comma
                    try
                    {
                        return Enum.Parse(type, data as string, true);
                    }
                    catch
                    {
                        return null;
                    }
                }

                // Try to cast the int/long/byte to a value within the enum.  
                var obj = Enum.ToObject(type, data);

                // It is possible that the cast will not be within the defined
                // values of the enumeration.  Use the IsDefined() method to determine if
                // there is a match, or when a bitmask enum is used, look for a comma in the
                // ToString() method.
                if (Enum.IsDefined(type, obj) | obj.ToString().Contains(","))
                {
                    return obj;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public static T FromDescription<T>(string enumName, T defaultValue)
        {
            if (string.IsNullOrEmpty(enumName))
            {
                return defaultValue;
            }

            var names = Enum.GetNames(typeof(T));

            foreach (var name in names)
            {
                var v = (T)Enum.Parse(typeof(T), name);

                var abbr = GetMemberDescription(v);

                if (abbr.EqualsIgnoreCase(enumName))
                {
                    return Parse(name, defaultValue);
                }
            }

            return defaultValue;
        }

        public static string GetMemberDescription<T>(T member)
        {
            if (ReferenceEquals(member, null))
            {
                throw new ArgumentNullException("member");
            }

            var name = member.ToString();

            return GetMemberDescription(typeof(T), name);
        }

        public static string GetMemberDescription(Type member, string name)
        {
            #region Parameter Check

            if (member == null)
            {
                throw new ArgumentNullException("member");
            }

            #endregion

            // Retrieve member matches
            var propertyInfo = member.GetProperty(name);

            // retrieve field matches
            var fieldInfo = member.GetField(name);

            // Save result in
            ICustomAttributeProvider provider = null;

            // If property match found
            if (propertyInfo != null)
            {
                provider = propertyInfo;
            }

            // If field match found
            if (fieldInfo != null)
            {
                provider = fieldInfo;
            }

            //Debug.Assert(
            //    provider != null,
            //    "Member (" + name + ") not found within type: " + member.Name
            //    );

            if (provider == null)
            {
                return name;
            }

            // Get custom metadata
            var arrMatches = provider.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (arrMatches.Length != 0)
            {
                return ((DescriptionAttribute)arrMatches[0]).Description;
            }

            return name;
        }

    }
}

