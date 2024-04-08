// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassDeclaration.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;

using System.Collections.Generic;
using Sundew.Injection.Generator.TypeSystem;

internal record ClassDeclaration(Type Type, bool IsSealed, IReadOnlyList<Member> Members, IReadOnlyList<AttributeDeclaration> AttributeDeclarations, IReadOnlyList<Type> Interfaces);