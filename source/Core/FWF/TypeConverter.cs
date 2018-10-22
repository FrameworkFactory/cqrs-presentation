using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace FWF
{
    public delegate object ConvertDelegate(object input, object defaultValue);

    public static class TypeConverter
    {
        private static readonly IDictionary<DoubleType, ConvertDelegate> _converters;

        private static readonly Dictionary<string, Type> _cSharpTypes;
        private static readonly Dictionary<string, DbType> _dbTypes;

        private static readonly Type _defaultType;
        private static readonly DbType _defaultDbType;

        private static volatile object _lockObject = new object();

        private class DoubleType
        {
            private readonly int _hash;

            public DoubleType(Type fromType, Type toType)
            {
                _hash = fromType.GetHashCode() ^ toType.GetHashCode() << 1;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(obj, null))
                {
                    return false;
                }

                return GetHashCode() == obj.GetHashCode();
            }

            public override int GetHashCode()
            {
                return _hash;
            }
        }

        static TypeConverter()
        {
            _converters = new Dictionary<DoubleType, ConvertDelegate>();

            // Add default converters
            AddConverter(typeof(string), typeof(int), DefaultConverters.StringToInt);
            AddConverter(typeof(int), typeof(string), DefaultConverters.IntToString);

            AddConverter(typeof(string), typeof(long), DefaultConverters.StringToLong);
            AddConverter(typeof(long), typeof(string), DefaultConverters.LongToString);

            AddConverter(typeof(string), typeof(decimal), DefaultConverters.StringToDecimal);
            AddConverter(typeof(decimal), typeof(string), DefaultConverters.DecimalToString);

            AddConverter(typeof(string), typeof(bool), DefaultConverters.StringToBoolean);
            AddConverter(typeof(bool), typeof(string), DefaultConverters.BooleanToString);

            AddConverter(typeof(string), typeof(DateTime), DefaultConverters.StringToDateTime);
            AddConverter(typeof(DateTime), typeof(string), DefaultConverters.DateTimeToString);

            AddConverter(typeof(string), typeof(TimeSpan), DefaultConverters.StringToTimeSpan);
            AddConverter(typeof(TimeSpan), typeof(string), DefaultConverters.TimeSpanToString);

            AddConverter(typeof(string), typeof(Uri), DefaultConverters.StringToUri);
            AddConverter(typeof(Uri), typeof(string), DefaultConverters.UriToString);

            AddConverter(typeof(string), typeof(Guid), DefaultConverters.StringToGuid);
            AddConverter(typeof(Guid), typeof(string), DefaultConverters.GuidToString);
            
            AddConverter(typeof(string), typeof(ushort), DefaultConverters.StringToUShort);
            AddConverter(typeof(ushort), typeof(string), DefaultConverters.UShortToString);

            AddConverter(typeof(string), typeof(Url), DefaultConverters.StringToUrl);
            AddConverter(typeof(Url), typeof(string), DefaultConverters.UrlToString);

            //
            _cSharpTypes = new Dictionary<string, Type>();
            _dbTypes = new Dictionary<string, DbType>();

            _defaultType = typeof(object);
            _defaultDbType = DbType.Object;

            _cSharpTypes.Add("varbinary", typeof(byte[]));
            _cSharpTypes.Add("binary", typeof(byte[]));
            _cSharpTypes.Add("image", typeof(byte[]));
            _cSharpTypes.Add("varchar", typeof(string));
            _cSharpTypes.Add("char", typeof(string));
            _cSharpTypes.Add("nvarchar", typeof(string));
            _cSharpTypes.Add("nchar", typeof(string));
            _cSharpTypes.Add("text", typeof(string));
            _cSharpTypes.Add("ntext", typeof(string));
            _cSharpTypes.Add("uniqueidentifier", typeof(Guid));
            _cSharpTypes.Add("rowversion", typeof(byte[]));
            _cSharpTypes.Add("bit", typeof(bool));
            _cSharpTypes.Add("tinyint", typeof(byte));
            _cSharpTypes.Add("smallint", typeof(Int16));
            _cSharpTypes.Add("int", typeof(int));
            _cSharpTypes.Add("bigint", typeof(long));
            _cSharpTypes.Add("smallmoney", typeof(decimal));
            _cSharpTypes.Add("numeric", typeof(decimal));
            _cSharpTypes.Add("decimal", typeof(decimal));
            _cSharpTypes.Add("real", typeof(Single));
            _cSharpTypes.Add("float", typeof(double));
            _cSharpTypes.Add("money", typeof(decimal));
            _cSharpTypes.Add("smalldatetime", typeof(DateTime));
            _cSharpTypes.Add("datetime", typeof(DateTime));
            _cSharpTypes.Add("sql_variant", typeof(object));
            _cSharpTypes.Add("timestamp", typeof(byte[]));

            _dbTypes.Add("varbinary", DbType.Binary);
            _dbTypes.Add("binary", DbType.Binary);
            _dbTypes.Add("image", DbType.Binary);
            _dbTypes.Add("varchar", DbType.String);
            _dbTypes.Add("char", DbType.String);
            _dbTypes.Add("nvarchar", DbType.String);
            _dbTypes.Add("nchar", DbType.String);
            _dbTypes.Add("text", DbType.String);
            _dbTypes.Add("ntext", DbType.String);
            _dbTypes.Add("uniqueidentifier", DbType.Guid);
            _dbTypes.Add("rowversion", DbType.Binary);
            _dbTypes.Add("bit", DbType.Boolean);
            _dbTypes.Add("tinyint", DbType.Byte);
            _dbTypes.Add("smallint", DbType.Int16);
            _dbTypes.Add("int", DbType.Int32);
            _dbTypes.Add("bigint", DbType.Int64);
            _dbTypes.Add("smallmoney", DbType.Decimal);
            _dbTypes.Add("numeric", DbType.Decimal);
            _dbTypes.Add("decimal", DbType.Decimal);
            _dbTypes.Add("real", DbType.Single);
            _dbTypes.Add("float", DbType.Double);
            _dbTypes.Add("money", DbType.Currency);
            _dbTypes.Add("smalldatetime", DbType.DateTime);
            _dbTypes.Add("datetime", DbType.DateTime);
            _dbTypes.Add("sql_variant", DbType.Object);
            _dbTypes.Add("timestamp", DbType.Binary);
        }

        public static void AddConverter(Type fromType, Type toType, ConvertDelegate converter)
        {
            var doubleType = new DoubleType(fromType, toType);

            lock (_lockObject)
            {
                if (!_converters.ContainsKey(doubleType))
                {
                    _converters.Add(doubleType, converter);
                }
            }
        }

        public static bool CanConvert(Type fromType, Type toType)
        {
            if (fromType == toType)
            {
                return true;
            }

            if (toType.IsAssignableFrom(fromType))
            {
                return true;
            }

            if (toType.Name == "Nullable`1")
            {
                var genericTypes = toType.GetGenericArguments();
                var genericType = genericTypes[0];

                toType = genericType;
            }

            if (toType.IsEnum)
            {
                return true;
            }

            var doubleType = new DoubleType(fromType, toType);

            return _converters.ContainsKey(doubleType);
        }

        public static T Convert<T>(object data, T defaultValue)
        {
            if (data == DBNull.Value)
            {
                return defaultValue;
            }
            if (ReferenceEquals(data, null))
            {
                return defaultValue;
            }

            var toType = typeof(T);
            var fromType = data.GetType();

            if (fromType == toType)
            {
                return (T)data;
            }

            if (toType.IsAssignableFrom(fromType))
            {
                return (T)data;
            }

            if (fromType.IsEnum)
            {
                if (typeof(T) == typeof(string))
                {
                    return (T)System.Convert.ChangeType(data.ToString(), typeof(string));
                }

                return (T)data;
            }

            // NOTE: If Nullable<T>, then
            if (toType.Name == "Nullable`1")
            {
                var genericTypes = toType.GetGenericArguments();
                var genericType = genericTypes[0];

                toType = genericType;
            }

            if (fromType.Name == "Nullable`1")
            {
                var genericTypes = fromType.GetGenericArguments();
                var genericType = genericTypes[0];

                // 
                fromType = genericType;
            }

            // NOTE: Provide a generic enum tranlation either by string or int
            if (toType.IsEnum)
            {
                // Try to convert to the object
                var obj = EnumManager.Parse(data, toType);

                // Return with a default value
                return EnumManager.Parse(obj, defaultValue);
            }

            // NOTE: This method's signature provides the from/to types for translation
            var dataType = data.GetType();
            var doubleType = new DoubleType(dataType, toType);

            if (_converters.ContainsKey(doubleType))
            {
                var converter = _converters[doubleType];

                try
                {
                    return (T)converter(data, defaultValue);
                }
                catch (Exception)
                {
                    //ErrorFormat(
                    //    ex,
                    //    "Unable to convert type {0} to {1} using {2}: {3}",
                    //    fromType,
                    //    toType,
                    //    converter.GetType().Name,
                    //    ex.Message
                    //    );

                    return defaultValue;
                }
            }

            Debug.WriteLine(
                "Unable to locate converter for from: {0}, to: {1}",
                data.GetType().Name,
                typeof(T).Name
                );

            // Worst-case senario, try System.Convert.ChangeType()
            try
            {
                // If destination type is a string, then use the ToString() method
                if (typeof(T) == typeof(string))
                {
                    data = System.Convert.ChangeType(data.ToString(), typeof(T));
                }
                else if (data is IConvertible)
                {
                    data = System.Convert.ChangeType(data, typeof(T));
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "Unable to convert type {0} to {1}",
                            data.GetType().Name,
                            typeof(T).Name
                            )
                        );
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    "Unable to convert type: {0} to {1}\r\n{2}",
                    data.GetType(),
                    typeof(T),
                    ex.RenderDetailString()
                    );

                return defaultValue;
            }

            return (T)data;
        }

        public static Type GetCSharpTypeFromDatabaseType(string sqlType)
        {
            if (_cSharpTypes.ContainsKey(sqlType))
            {
                return _cSharpTypes[sqlType];
            }

            return _defaultType;
        }

        public static DbType GetDbTypeFromDatabaseType(string sqlType)
        {
            if (_dbTypes.ContainsKey(sqlType))
            {
                return _dbTypes[sqlType];
            }

            return _defaultDbType;
        }

    }
}

