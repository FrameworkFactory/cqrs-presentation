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



