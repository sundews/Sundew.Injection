// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnCreateMethodDeclaration.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;

using System.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

internal record OnCreateMethodDeclaration(string Name, ImmutableList<ParameterDeclaration> Parameters, DefiniteType ReturnType);