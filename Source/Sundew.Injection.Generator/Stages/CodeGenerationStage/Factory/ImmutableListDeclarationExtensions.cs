// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImmutableListDeclarationExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

using System;
using System.Collections.Immutable;
using System.Linq;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;
using Sundew.Injection.Generator.TypeSystem;
using Type = Sundew.Injection.Generator.TypeSystem.Type;

internal static class ImmutableListDeclarationExtensions
{
    public static (ImmutableList<TDeclaration> Declarations, bool WasAdded, TDeclaration Declaration) GetOrAdd<TDeclaration>(
        this ImmutableList<TDeclaration> declarations,
        string name,
        DefiniteType type,
        Func<string, DefiniteType, TDeclaration> createDeclarationFunc)
        where TDeclaration : struct, IDeclaration
    {
        var conflictingDeclaration = declarations.FirstOrDefault(x => x.Name == name);
        if (!Equals(conflictingDeclaration, default(TDeclaration)))
        {
            if (conflictingDeclaration.Type.Equals(type))
            {
                return (declarations, false, conflictingDeclaration);
            }
        }

        var declaration = createDeclarationFunc(name, type);
        return (declarations.Add(declaration), true, declaration);
    }

    public static (ImmutableList<TDeclaration> Declarations, TDeclaration Declaration)
        AddUnique<TDeclaration>(
            this ImmutableList<TDeclaration> declarations,
            string name,
            Type type,
            Func<string, int, string> proposeNameFunc,
            Func<string, TDeclaration> createDeclarationFunc)
    where TDeclaration : struct, IDeclaration
    {
        var result = declarations.PrivateEnsureDeclaration(
            name,
            type,
            1,
            true,
            proposeNameFunc,
            createDeclarationFunc);
        return (result.Declarations, result.Declaration);
    }

    public static (ImmutableList<TDeclaration> Declarations, bool WasAdded, TDeclaration Declaration)
        GetOrAdd<TDeclaration>(
            this ImmutableList<TDeclaration> declarations,
            string name,
            Type type,
            Func<string, TDeclaration> createDeclarationFunc)
    where TDeclaration : struct, IDeclaration
    {
        return declarations.PrivateEnsureDeclaration(name, type, 1, false, (s, _) => s, createDeclarationFunc);
    }

    private static (ImmutableList<TDeclaration> Declarations, bool WasAdded, TDeclaration Declaration)
        PrivateEnsureDeclaration<TDeclaration>(
            this ImmutableList<TDeclaration> declarations,
            string name,
            Type type,
            int proposedCount,
            bool mustAdd,
            Func<string, int, string> proposeNameFunc,
            Func<string, TDeclaration> createDeclarationFunc)
    where TDeclaration : struct, IDeclaration
    {
        var conflictingDeclaration = declarations.FirstOrDefault(x => x.Name == name);
        if (!Equals(conflictingDeclaration, default(TDeclaration)))
        {
            if (!mustAdd && conflictingDeclaration.Type.Equals(type))
            {
                return (declarations, false, conflictingDeclaration);
            }

            return declarations.PrivateEnsureDeclaration(
                proposeNameFunc(name, proposedCount),
                type,
                proposedCount + 1,
                mustAdd,
                proposeNameFunc,
                createDeclarationFunc);
        }

        var declaration = createDeclarationFunc(name);
        return (declarations.Add(declaration), true, declaration);
    }
}