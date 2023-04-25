// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryImplementation.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;

using System.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;

internal readonly record struct FactoryImplementation(
    ImmutableList<FieldDeclaration> Fields,
    Constructor Constructor,
    ImmutableList<DeclaredMethodImplementation> CreateMethods,
    ImmutableList<DeclaredMethodImplementation> FactoryMethods,
    ImmutableList<DeclaredDisposeMethodImplementation> DisposeForMethodImplementations,
    ImmutableList<Statement> DisposeMethodStatements)
{
    public FactoryImplementation()
        : this(
            ImmutableList<FieldDeclaration>.Empty,
            new Constructor(
                ImmutableList<ParameterDeclaration>.Empty,
                ImmutableList<Statement>.Empty,
                false),
            ImmutableList<DeclaredMethodImplementation>.Empty,
            ImmutableList<DeclaredMethodImplementation>.Empty,
            ImmutableList<DeclaredDisposeMethodImplementation>.Empty,
            ImmutableList<Statement>.Empty)
    {
    }
}