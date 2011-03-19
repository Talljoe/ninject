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
using System.Linq;

using Ninject.Activation;
using Ninject.Components;
using Ninject.Infrastructure.Language;

#endregion

namespace Ninject.Planning.Bindings.Resolvers
{

    /// <summary>
    /// Last chance resolver that can respond to requests for various enumerable types.
    /// </summary>
    public class EnumerableLastChanceResolver : NinjectComponent, ILastChanceResolver
    {
        private readonly IKernel kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerableLastChanceResolver"/> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public EnumerableLastChanceResolver(IKernel kernel)
        {
            this.kernel = kernel;
        }

        /// <summary>
        /// Attempts to reesolve the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Any objects that match the request if it can be matched; otherwise an empty string.</returns>
        public IEnumerable<object> Resolve(IRequest request)
        {
            Func<IEnumerable, Type, IEnumerable> transform = (r, _) => r;
            Type service;
            var enumerableType = GetEnumerableType(request.Service);
            if(enumerableType == EnumerableType.None)
            {
                return Enumerable.Empty<object>();
            }
            switch (enumerableType)
            {
                case EnumerableType.Array:
                    service = request.Service.GetElementType();
                    transform = (r, s) => r.ToArraySlow(s);
                    break;
                case EnumerableType.List:
                case EnumerableType.Enumerable:
                    service = request.Service.GetGenericArguments()[0];
                    if (enumerableType == EnumerableType.List)
                    {
                        transform = (r, s) => r.ToListSlow(s);
                    }
                    break;
                default:
                    return null;
            }

            var subRequest = (request.Target != null)
                                 ? request.CreateChild(service, request.ParentContext, request.Target)
                                 : kernel.CreateRequest(
                                     service, request.Constraint, request.Parameters, true, false);
            subRequest.IsOptional = true;
            return new[]{transform(kernel.Resolve(subRequest).CastSlow(service), service)};
        }

        private static EnumerableType GetEnumerableType(Type service)
        {
            if (service.IsGenericType)
            {
                var gtd = service.GetGenericTypeDefinition();
                if (gtd == typeof(List<>) || gtd == typeof(IList<>) || gtd == typeof(ICollection<>))
                {
                    return EnumerableType.List;
                }
                if (gtd == typeof(IEnumerable<>))
                {
                    return EnumerableType.Enumerable;
                }
            }

            return service.IsArray ? EnumerableType.Array : EnumerableType.None;
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