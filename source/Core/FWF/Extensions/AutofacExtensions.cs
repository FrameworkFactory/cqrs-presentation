using Autofac;
using Autofac.Builder;
using Autofac.Features.Scanning;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FWF
{
    public static class AutofacExtensions
    {

        public static IEnumerable<T> ResolveAll<T>(this IComponentContext componentContext)
        {
            Type genericType = typeof(IEnumerable<>).MakeGenericType(typeof(T));

            return componentContext.Resolve(genericType) as IEnumerable<T>;
        }

        public static IEnumerable<object> ResolveAll(this IComponentContext componentContext, Type type)
        {
            Type genericType = typeof(IEnumerable<>).MakeGenericType(type);

            return componentContext.Resolve(genericType) as IEnumerable<object>;
        }

    }
}



