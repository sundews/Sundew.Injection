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
    ImmutableList<DeclaredMethodImplementation> CreateMethods,
    ImmutableList<PropertyDeclaration> Properties,
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
            ImmutableList<DeclaredMethodImplementation>.Empty,
            ImmutableList<PropertyDeclaration>.Empty,
            ImmutableList<DeclaredMethodImplementation>.Empty,
            ImmutableList<DeclaredDisposeMethodImplementation>.Empty,
            ImmutableList<DeclaredMethodImplementation>.Empty)
    {
    }
}