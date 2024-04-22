// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldDeclaration.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;

using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

internal sealed record PropertyDeclaration(Type Type, string Name, ValueList<AttributeDeclaration> Attributes) : MemberDeclaration, IDeclaration;