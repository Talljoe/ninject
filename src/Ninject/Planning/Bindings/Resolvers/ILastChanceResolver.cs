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
using System.Collections.Generic;
using Ninject.Activation;
using Ninject.Components;
#endregion
namespace Ninject.Planning.Bindings.Resolvers
{
    /// <summary>
    /// Interface that handles returnining an object when the kernel has failed.
    /// </summary>
    public interface ILastChanceResolver : INinjectComponent
    {
        /// <summary>
        /// Attempts to reesolve the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Any objects that match the request if it can be matched; otherwise an empty string.</returns>
        IEnumerable<object> Resolve(IRequest request);
    }
}