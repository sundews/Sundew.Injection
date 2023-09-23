// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainingTypeMetadata.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using Sundew.Base.Primitives.Computation;

internal readonly record struct TypeMetadata(O<Method> DefaultConstructor, bool ImplementsIEnumerable, bool HasLifetime, bool IsValueType);