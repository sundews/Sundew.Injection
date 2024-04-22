// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryImplementation.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model;

using System.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Statement = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.Statement;

internal readonly record struct FactoryImplementation(
    ImmutableList<FieldDeclaration> Fields,
    Constructor Constructor,
    ImmutableList<DeclaredPropertyImplementation> Properties,
    ImmutableList<DeclaredMethodImplementation> CreateMethods,
    ImmutableList<DeclaredMethodImplementation> FactoryMethods,
    ImmutableList<DeclaredDisposeMethodImplementation> DisposeMethodImplementations,
    ImmutableList<DeclaredMethodImplementation> PrivateCreateMethods)
{
    public FactoryImplementation()
        : this(
            ImmutableList<FieldDeclaration>.Empty,
            new Constructor(
                ImmutableList<ParameterDeclaration>.Empty,
                ImmutableList<Statement>.Empty),
            ImmutableList<DeclaredPropertyImplementation>.Empty,
            ImmutableList<DeclaredMethodImplementation>.Empty,
            ImmutableList<DeclaredMethodImplementation>.Empty,
            ImmutableList<DeclaredDisposeMethodImplementation>.Empty,
            ImmutableList<DeclaredMethodImplementation>.Empty)
    {
    }
}