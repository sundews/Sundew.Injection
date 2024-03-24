// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefiniteClosedEnumerableType.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

internal record struct DefiniteClosedEnumerableType(
    string Name,
    string Namespace,
    string AssemblyName,
    DefiniteTypeArgument TypeArgument,
    bool IsValueType);