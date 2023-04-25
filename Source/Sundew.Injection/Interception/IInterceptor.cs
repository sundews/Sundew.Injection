// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInterceptor.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Interception
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public delegate IReturn Invoke(IMethodCall methodCall, object target);

    public interface IInterceptor
    {
        void BeforeInvoke(IMethodCall methodCall, object target);

        IReturn AfterInvoke(IMethodCall methodCall, object target);
    }

    public interface IMethodCall
    {
        MethodBase MethodBase { get; }

        string Name { get; }

        IReadOnlyList<IArgument> Arguments { get; }
    }

    public interface IReturn
    {
        object[] OutValues { get; }

        object Value { get; set; }

        Exception Exception { get; }
    }

    public interface IArgument
    {
        string Name { get; }

        Type Type { get; }

        object Value { get; }
    }
}