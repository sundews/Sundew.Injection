// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenGenericType.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using Sundew.Base.Collections.Immutable;

internal sealed record OpenGenericType(string Name, string Namespace, string AssemblyName, ValueArray<TypeParameter> TypeParameters, bool IsValueType) : Symbol(Name);