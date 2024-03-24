// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterfaceDeclaration.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;

using System.Collections.Generic;
using Sundew.Injection.Generator.TypeSystem;

internal record InterfaceDeclaration(DefiniteType Type, IReadOnlyList<DefiniteType> InterfaceTypes, IReadOnlyList<AttributeDeclaration> AttributeDeclarations, IReadOnlyList<MethodDeclaration> Members);