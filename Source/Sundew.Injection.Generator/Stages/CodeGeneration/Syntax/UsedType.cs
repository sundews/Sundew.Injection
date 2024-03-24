// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsedType.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;

using Sundew.Injection.Generator.TypeSystem;

internal readonly record struct UsedType(DefiniteType Type, bool CanHaveDefaultValue = false);