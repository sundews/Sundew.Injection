// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImmutableListDeclarationExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage;

using System;
using System.Collections.Immutable;
using System.Linq;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Type = Sundew.Injection.Generator.TypeSystem.Type;

internal static class ImmutableListDeclarationExtensions
{
    public static (ImmutableList<TDeclaration> Declarations, bool WasAdded, TDeclaration Declaration) GetOrAdd<TDeclaration>(
        this ImmutableList<TDeclaration> declarations,
        string name,
        Type type,
        Func<string, Type, TDeclaration> createDeclarationFunc)
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
        var result = declarations.PrivateGetOrCreate(
            name,
            type,
            1,
            true,
            proposeNameFunc,
            createDeclarationFunc);
        return (result.WasCreated ? declarations.Add(result.Declaration) : declarations, result.Declaration);
    }

    public static (bool WasCreated, TDeclaration Declaration)
        GetOrCreate<TDeclaration>(
            this ImmutableList<TDeclaration> declarations,
            string name,
            Type type,
            Func<string, TDeclaration> createDeclarationFunc)
        where TDeclaration : struct, IDeclaration
    {
        return declarations.PrivateGetOrCreate(name, type, 1, false, (s, _) => s, createDeclarationFunc);
    }

    public static (ImmutableList<TDeclaration> Declarations, bool WasAdded, TDeclaration Declaration)
        GetOrAdd<TDeclaration>(
            this ImmutableList<TDeclaration> declarations,
            string name,
            Type type,
            Func<string, TDeclaration> createDeclarationFunc)
    where TDeclaration : struct, IDeclaration
    {
        var result = declarations.PrivateGetOrCreate(name, type, 1, false, (s, _) => s, createDeclarationFunc);
        return (result.WasCreated ? declarations.Add(result.Declaration) : declarations, result.WasCreated, result.Declaration);
    }

    private static (bool WasCreated, TDeclaration Declaration)
        PrivateGetOrCreate<TDeclaration>(
            this ImmutableList<TDeclaration> declarations,
            string name,
            Type type,
            int proposedCount,
            bool mustCreate,
            Func<string, int, string> proposeNameFunc,
            Func<string, TDeclaration> createDeclarationFunc)
        where TDeclaration : struct, IDeclaration
    {
        var conflictingDeclaration = declarations.FirstOrDefault(x => x.Name == name);
        if (!Equals(conflictingDeclaration, default(TDeclaration)))
        {
            if (!mustCreate && conflictingDeclaration.Type.Equals(type))
            {
                return (false, conflictingDeclaration);
            }

            return declarations.PrivateGetOrCreate(
                proposeNameFunc(name, proposedCount),
                type,
                proposedCount + 1,
                mustCreate,
                proposeNameFunc,
                createDeclarationFunc);
        }

        var declaration = createDeclarationFunc(name);
        return (true, declaration);
    }
}