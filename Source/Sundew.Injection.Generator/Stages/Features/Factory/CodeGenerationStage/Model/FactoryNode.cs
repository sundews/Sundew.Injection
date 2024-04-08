// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model;

using System.Collections.Immutable;
using Expression = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.Expression;

internal readonly record struct FactoryNode(
    in FactoryImplementation FactoryImplementation,
    in MethodImplementation CreateMethod,
    ImmutableList<Expression> DependantArguments);