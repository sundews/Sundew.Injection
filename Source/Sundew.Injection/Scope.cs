// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Scope.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection;

using System;
using System.Linq.Expressions;

public abstract class Scope
{
    private Scope()
    {
    }

    public static Scope Auto { get; } = new AutoScope();

    public static Scope NewInstance { get; } = new NewInstanceScope();

    public static Scope SingleInstancePerRequest { get; } = new SingleInstancePerRequestScope();

    public static Scope SingleInstancePerFactory() => new SingleInstancePerFactoryScope();

    public static Scope SingleInstancePerFuncResult(Expression<Func<object>> func) => new SingleInstancePerFuncResultScope(func);

    internal sealed class AutoScope : Scope
    {
    }

    internal sealed class NewInstanceScope : Scope
    {
    }

    internal sealed class SingleInstancePerRequestScope : Scope
    {
    }

    internal sealed class SingleInstancePerFuncResultScope : Scope
    {
        public SingleInstancePerFuncResultScope(Expression<Func<object>> func)
        {
            this.Func = func;
        }

        public Expression<Func<object>> Func { get; }
    }

    internal sealed class SingleInstancePerFactoryScope : Scope;
}