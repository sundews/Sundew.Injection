﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NameHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Syntax;

using System;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;
using Sundew.Injection.Generator.TypeSystem;

/// <summary>
/// Helper methods for names.
/// </summary>
internal static class NameHelper
{
    /// <summary>
    /// Gets the name of the unique.
    /// </summary>
    /// <param name="baseName">Name of the base.</param>
    /// <param name="parentCreationNode">The parent creation node.</param>
    /// <returns>The name string.</returns>
    public static string GetUniqueName(string baseName, IInjectionNode? parentCreationNode)
    {
        if (parentCreationNode == null)
        {
            return baseName.Uncapitalize();
        }

        return $"{baseName}For{parentCreationNode.Name}".Uncapitalize();
    }

    public static string GetVariableNameForType(DefiniteType type)
    {
        const string Empty = "";
        const string Dot = ".";

        var name = type.Name.AsSpan();
        if (IsInterfaceName(name))
        {
            name = name.Slice(1);
        }

        switch (type)
        {
            case DefiniteArrayType:
                const string arraySign = "[]";
                const string arrayName = "Array";
                if (name.EndsWith(arraySign.AsSpan()))
                {
                    name = name.Slice(0, name.Length - arraySign.Length);
                }

                return name.ToString().Replace(Dot, Empty).Uncapitalize() + arrayName;
            case DefiniteBoundGenericType:
                break;
            case NamedType:
                break;
        }

        return name.ToString().Replace(Dot, Empty).Uncapitalize();
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
        return name[0] == 'I' && char.IsUpper(name[1]);
    }
}