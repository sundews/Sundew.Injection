﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NameHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;

using System;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;
using Sundew.Injection.Generator.TypeSystem;
using Type = Sundew.Injection.Generator.TypeSystem.Type;

/// <summary>
/// Helper methods for names.
/// </summary>
internal static class NameHelper
{
    /// <summary>
    /// Gets the name scoped to the dependant.
    /// </summary>
    /// <param name="injectionNode">The injection node.</param>
    /// <returns>The name string.</returns>
    public static string GetDependantScopedName(NewInstanceInjectionNode injectionNode)
    {
        var name = injectionNode.TargetType.GetTypeName();
        return GetDependantScopedName(name, injectionNode.DependantName);
    }

    /// <summary>
    /// Gets the name scoped to the dependant.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="dependantName">The dependant name.</param>
    /// <returns>The name string.</returns>
    public static string GetDependantScopedName(string name, string? dependantName)
    {
        if (dependantName.IsNullOrEmpty())
        {
            return name.Uncapitalize();
        }

        return $"{name}For{dependantName}".Uncapitalize();
    }

    public static string GetIdentifierNameForType(Type type)
    {
        var name = type.Name.AsSpan();
        if (IsInterfaceName(name))
        {
            name = name.Slice(1);
        }

        switch (type)
        {
            case ArrayType:
                const string arraySign = "[]";
                const string arrayName = "Array";
                if (name.EndsWith(arraySign.AsSpan()))
                {
                    name = name.Slice(0, name.Length - arraySign.Length);
                }

                return name.ToString().Uncapitalize() + arrayName;
            case ClosedGenericType:
                break;
            case NestedType:
                break;
            case NamedType:
                break;
        }

        return name.ToString().Uncapitalize();
    }

    public static string GetFactoryMethodName(string name)
    {
        var span = name.AsSpan();
        if (IsInterfaceName(span))
        {
            return span.Slice(1).ToString();
        }

        return name;
    }

    private static bool IsInterfaceName(ReadOnlySpan<char> name)
    {
        return name.Length > 1 && name[0] == 'I' && char.IsUpper(name[1]);
    }
}