using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace FWF
{
    [DebuggerStepThrough]
    public static class ObjectExtensions
    {

        public static bool IsNull(this object input)
        {
            return ReferenceEquals(input, null);
        }

        public static bool IsNotNull(this object input)
        {
            return !ReferenceEquals(input, null);
        }

        public static T Cast<T>(this object obj, T defaultValue)
        {
            return TypeConverter.Convert(obj, defaultValue);
        }

        public static bool CanCast<T>(this object obj, T defaultValue)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            return TypeConverter.CanConvert(obj.GetType(), typeof(T));
        }

        public static IDictionary<string, object> ParseProperties(this object obj)
        {
            var listProperties = new Dictionary<string, object>();

            var properties = TypeDescriptor.GetProperties(obj);

            foreach (PropertyDescriptor propertyDescriptor in properties)
            {
                var value = propertyDescriptor.GetValue(obj);

                listProperties.Add(propertyDescriptor.Name, value);
            }

            return listProperties;
        }


    }
}



