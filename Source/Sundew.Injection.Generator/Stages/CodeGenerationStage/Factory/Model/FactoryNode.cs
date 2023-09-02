// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;

using System.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;

internal readonly record struct FactoryNode(
    in FactoryImplementation FactoryImplementation,
    in MethodImplementation CreateMethod,
    ImmutableList<Expression> DependeeArguments);