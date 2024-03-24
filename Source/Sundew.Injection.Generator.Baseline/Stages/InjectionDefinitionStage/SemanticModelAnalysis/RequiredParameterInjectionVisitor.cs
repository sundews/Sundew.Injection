// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequiredParameterInjectionVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Base;

internal class RequiredParameterInjectionVisitor : CSharpSyntaxWalker
{
    private readonly CompiletimeInjectionDefinitionBuilder compiletimeInjectionDefinitionBuilder;

    public RequiredParameterInjectionVisitor(CompiletimeInjectionDefinitionBuilder compiletimeInjectionDefinitionBuilder)
    {
        this.compiletimeInjectionDefinitionBuilder = compiletimeInjectionDefinitionBuilder;
    }

    public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        this.compiletimeInjectionDefinitionBuilder.RequiredParameterInjection = node.Name.Identifier.ValueText.ParseEnum<Inject>();
    }
}