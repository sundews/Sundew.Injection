// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Scope.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using Sundew.DiscriminatedUnions;

[DiscriminatedUnion]
internal abstract record Scope
{
    [CaseType(typeof(AutoScope))]
    public static Scope Auto { get; } = new AutoScope();

    [CaseType(typeof(NewInstanceScope))]
    public static Scope NewInstance { get; } = new NewInstanceScope();

    [CaseType(typeof(SingleInstancePerRequestScope))]
    public static Scope SingleInstancePerRequest { get; } = new SingleInstancePerRequestScope();

    [CaseType(typeof(SingleInstancePerFactoryScope))]
    public static Scope SingleInstancePerFactory { get; } = new SingleInstancePerFactoryScope();

    [CaseType(typeof(SingleInstancePerFuncResultScope))]
    public static Scope SingleInstancePerFuncResult(Method method) => new SingleInstancePerFuncResultScope(method);

    internal sealed record AutoScope : Scope;

    internal sealed record NewInstanceScope : Scope;

    internal sealed record SingleInstancePerRequestScope : Scope;

    internal sealed record SingleInstancePerFuncResultScope(Method Method) : Scope;

    internal sealed record SingleInstancePerFactoryScope : Scope;
}