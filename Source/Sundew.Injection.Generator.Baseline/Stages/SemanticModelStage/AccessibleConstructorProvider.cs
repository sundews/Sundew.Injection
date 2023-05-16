// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccessibleConstructorProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.SemanticModelStage;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static class AccessibleConstructorProvider
{
    public static IncrementalValuesProvider<SyntaxNode> SetupAccessibleConstructorStage(this SyntaxValueProvider syntaxProvider)
    {
        return syntaxProvider.CreateSyntaxProvider(
            static (syntaxNode, _) => IsConstructor(syntaxNode),
            static (generatorContextSyntax, _) => GetConstructorSyntax(generatorContextSyntax)).Where(x => x != null).Select((x, y) => x!);
    }

    private static SyntaxNode? GetConstructorSyntax(GeneratorSyntaxContext generatorContextSyntax)
    {
        var invocationSymbolInfo = generatorContextSyntax.SemanticModel.GetSymbolInfo(generatorContextSyntax.Node);
        if (invocationSymbolInfo.Symbol is IMethodSymbol)
        {
            return generatorContextSyntax.Node;
        }

        return null;
    }

    private static bool IsConstructor(SyntaxNode syntaxNode)
    {
        if (syntaxNode is ConstructorDeclarationSyntax)
        {
            return true;
        }

        return false;
    }
}