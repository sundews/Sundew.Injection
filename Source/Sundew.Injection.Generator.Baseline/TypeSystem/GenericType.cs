// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericType.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using Sundew.Base.Collections.Immutable;

public sealed record GenericType(string Name, string Namespace, string AssemblyName, ValueArray<TypeParameter> TypeParameters);