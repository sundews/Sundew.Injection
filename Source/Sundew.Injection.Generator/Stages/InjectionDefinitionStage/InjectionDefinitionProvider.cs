// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionDefinitionProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;
using Sundew.Injection.Generator.TypeSystem;

internal static class InjectionDefinitionProvider
{
    public static
        IncrementalValuesProvider<R<InjectionDefinition,
            ValueList<Diagnostic>>> SetupInjectionDefinitionStage(this SyntaxValueProvider syntaxValueProvider)
    {
        return syntaxValueProvider.CreateSyntaxProvider(
            static (syntaxNode, _) => IsInjectionDeclaration(syntaxNode),
            static (generatorContextSyntax, cancellationToken) => GetInjectionDefinition(generatorContextSyntax.SemanticModel, cancellationToken));
    }

    internal static R<InjectionDefinition, ValueList<Diagnostic>> GetInjectionDefinition(SemanticModel injectionDeclarationSemanticModel, CancellationToken cancellationToken)
    {
        try
        {
            var knownAnalysisTypesResult = KnownAnalysisTypes.Get(injectionDeclarationSemanticModel.Compilation);
            if (!knownAnalysisTypesResult.TryGet(out var knownAnalysisTypes, out var errors))
            {
                return R.Error(errors);
            }

            var compiletimeInjectionDefinitionBuilder = new CompiletimeInjectionDefinitionBuilder(string.Empty);
            var typeFactory = new TypeFactory(knownAnalysisTypes);
            var injectionDeclarationVisitor = new InjectionDeclarationVisitor(new AnalysisContext(injectionDeclarationSemanticModel, knownAnalysisTypes, typeFactory, compiletimeInjectionDefinitionBuilder), cancellationToken);
            injectionDeclarationVisitor.Visit(injectionDeclarationSemanticModel.SyntaxTree.GetRoot());
            return compiletimeInjectionDefinitionBuilder.Build();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return R.Error(ImmutableArray.Create(Diagnostic.Create(Diagnostics.UnknownError, default, e.ToString())).ToValueList());
        }
    }

    private static bool IsInjectionDeclaration(SyntaxNode syntaxNode)
    {
        if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
        {
            if (classDeclarationSyntax.BaseList?.Types.Any(x =>
                    x.Type is IdentifierNameSyntax identifierNameSyntax &&
                    identifierNameSyntax.Identifier.Text == nameof(IInjectionDeclaration)) == true)
            {
                return true;
            }
        }

        return false;
    }
}