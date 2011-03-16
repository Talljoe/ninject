#region License
//
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
//
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
//
#endregion
#region Using Directives
using System;
using System.Collections;
using System.Collections.Generic;
using Ninject.Activation;
using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Language;

#endregion

namespace Ninject.Planning.Bindings.Resolvers
{
    /// <summary>
    /// A resolver for creating IEnumerables from bound classes.
    /// </summary>
    public class EnumerableResolver : NinjectComponent, IMissingBindingResolver
    {
        /// <summary>
        /// Returns any bindings from the specified collection that match the specified request.
        /// </summary>
        /// <param name="bindings">The multimap of all registered bindings.</param>
        /// <param name="request">The request in question.</param>
        /// <returns>The series of matching bindings.</returns>
        public IEnumerable<IBinding> Resolve(Multimap<Type, IBinding> bindings, IRequest request)
        {
            var service = request.Service;
            if(GetEnumerableType(service) != EnumerableType.None)
            {
                yield return new Binding(service)
                                 {
                                     ProviderCallback = ctx => new EnumerableProvider(service),
                                 };
            }
        }

        private static EnumerableType GetEnumerableType(Type service)
        {
            if (service.IsGenericType)
            {
                var gtd = service.GetGenericTypeDefinition();
                if (gtd == typeof (List<>) || gtd == typeof (IList<>) || gtd == typeof (ICollection<>))
                {
                    return EnumerableType.List;
                }
                if (gtd == typeof (IEnumerable<>))
                {
                    return EnumerableType.Enumerable;
                }
            }

            return service.IsArray ? EnumerableType.Array : EnumerableType.None;
        }

        private class EnumerableProvider : IProvider
        {
            private readonly Type type;

            public EnumerableProvider(Type type)
            {
                this.type = type;
            }

            public Type Type
            {
                get { return this.type; }
            }

            public object Create(IContext context)
            {
                Func<IEnumerable, Type, IEnumerable> transform = (r, _) => r;
                Type service;
                var enumerableType = GetEnumerableType(this.Type);
                switch(enumerableType)
                {
                    case EnumerableType.Array:
                        service = this.Type.GetElementType();
                        transform = (r, s) => r.ToArraySlow(s);
                        break;
                    case EnumerableType.List:
                    case EnumerableType.Enumerable:
                        service = this.Type.GetGenericArguments()[0];
                        if(enumerableType == EnumerableType.List)
                        {
                            transform = (r, s) => r.ToListSlow(s);
                        }
                        break;
                    default:
                        return null;
                }

                var request = context.Kernel.CreateRequest(service, context.Request.Constraint, context.Request.Parameters, true, false);
                return transform(context.Kernel.Resolve(request).CastSlow(service), service);
            }
        }

        private enum EnumerableType
        {
            None,
            Array,
            List,
            Enumerable,
        }
    }
}