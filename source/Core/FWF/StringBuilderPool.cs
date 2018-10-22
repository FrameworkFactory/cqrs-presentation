using System;
using System.Text;

namespace FWF
{
    public static class StringBuilderPool
    {
        private const int MAX_BUILDER_SIZE = 360;

        [ThreadStatic]
        private static StringBuilder CachedInstance;

        public static StringBuilder Acquire(int capacity = 16)
        {
            if (capacity <= 360)
            {
                StringBuilder cachedInstance = StringBuilderPool.CachedInstance;
                if (cachedInstance != null && capacity <= cachedInstance.Capacity)
                {
                    StringBuilderPool.CachedInstance = null;
                    cachedInstance.Clear();
                    return cachedInstance;
                }
            }
            return new StringBuilder(capacity);
        }

        public static void Release(StringBuilder sb)
        {
            if (sb.Capacity <= 360)
            {
                StringBuilderPool.CachedInstance = sb;
            }
        }

        public static string GetStringAndRelease(StringBuilder sb)
        {
            string result = sb.ToString();
            StringBuilderPool.Release(sb);
            return result;
        }

    }
}

