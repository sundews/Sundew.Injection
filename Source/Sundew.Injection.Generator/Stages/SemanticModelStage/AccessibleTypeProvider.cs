// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccessibleConstructorProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.SemanticModelStage;

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Base;

internal static class AccessibleTypeProvider
{
    private static readonly bool[] ValidTypes =
    [
        false, // None = 0,
        true, // System_Object = 1,
        true, // System_Enum = 2,
        true, // System_MulticastDelegate = 3,
        true, // System_Delegate = 4,
        true, // System_ValueType = 5,
        false, // System_Void = 6,
        false, // System_Boolean = 7,
        false, // System_Char = 8,
        false, // System_SByte = 9,
        false, // System_Byte = 10,
        false, // System_Int16 = 11,
        false, // System_UInt16 = 12,
        false, // System_Int32 = 13,
        false, // System_UInt32 = 14,
        false, // System_Int64 = 15,
        false, // System_UInt64 = 16,
        false, // System_Decimal = 17,
        false, // System_Single = 18,
        false, // System_Double = 19,
        false, // System_String = 20,
        false, // System_IntPtr = 21,
        false, // System_UIntPtr = 22,
        true, // System_Array = 23,
        true, // System_Collections_IEnumerable = 24,
        true, // System_Collections_Generic_IEnumerable_T = 25, // Note: IEnumerable<int> (i.e. constructed type) has no special type
        true, // System_Collections_Generic_IList_T = 26,
        true, // System_Collections_Generic_ICollection_T = 27,
        true, // System_Collections_IEnumerator = 28,
        true, // System_Collections_Generic_IEnumerator_T = 29,
        true, // System_Collections_Generic_IReadOnlyList_T = 30,
        true, // System_Collections_Generic_IReadOnlyCollection_T = 31,
        true, // System_Nullable_T = 32,
        false, // System_DateTime = 33,
        false, // System_Runtime_CompilerServices_IsVolatile = 34,
        false, // System_IDisposable = 35,
        false, // System_TypedReference = 36,
        false, // System_ArgIterator = 37,
        false, // System_RuntimeArgumentHandle = 38,
        false, // System_RuntimeFieldHandle = 39,
        false, // System_RuntimeMethodHandle = 40,
        false, // System_RuntimeTypeHandle = 41,
        false, // System_IAsyncResult = 42,
        false, // System_AsyncCallback = 43,
        false, // System_Runtime_CompilerServices_RuntimeFeature = 44,
        false, // System_Runtime_CompilerServices_PreserveBaseOverridesAttribute = 45,
        false, // Count];
    ];

    public static IncrementalValuesProvider<SyntaxNode> SetupAccessibleTypeStage(this SyntaxValueProvider syntaxProvider)
    {
        return syntaxProvider.CreateSyntaxProvider(
            static (syntaxNode, _) => IsConstructor(syntaxNode),
            static (generatorContextSyntax, _) => GetConstructorSyntax(generatorContextSyntax)).Where(x => x != null).Select((x, y) => x!);
    }

    private static SyntaxNode? GetConstructorSyntax(GeneratorSyntaxContext generatorContextSyntax)
    {
        var typeSymbolInfo = generatorContextSyntax.SemanticModel.GetSymbolInfo(generatorContextSyntax.Node);
        if (typeSymbolInfo.Symbol is INamedTypeSymbol namedTypeSymbol && HasRelevantTargets(namedTypeSymbol))
        {
            return generatorContextSyntax.Node;
        }

        return default;
    }

    private static bool HasRelevantTargets(INamedTypeSymbol namedTypeSymbol)
    {
        return namedTypeSymbol.GetMembers().Any(x =>
            {
                switch (x)
                {
                    case IMethodSymbol methodSymbol when IsDeclaredAccessible(methodSymbol.DeclaredAccessibility)
                                                         && HasValidReturnType(methodSymbol.ReturnType):
                    case IPropertySymbol propertySymbol when IsDeclaredAccessible(propertySymbol.DeclaredAccessibility)
                                                             && propertySymbol.GetMethod.HasValue()
                                                             && HasValidReturnType(propertySymbol.GetMethod.ReturnType):
                        return true;
                    default:
                        return false;
                }
            });
    }

    private static bool HasValidReturnType(ITypeSymbol typeSymbol)
    {
        return ValidTypes[(int)typeSymbol.SpecialType];
    }

    private static bool IsDeclaredAccessible(Accessibility accessibility)
    {
        return accessibility == Accessibility.Public || accessibility == Accessibility.Internal;
    }

    private static bool IsConstructor(SyntaxNode syntaxNode)
    {
        if (syntaxNode is TypeDeclarationSyntax typeDeclarationSyntax && typeDeclarationSyntax.Members.Any(x => x.Modifiers.Any(x => IsAccessible(x.Kind()))))
        {
            return true;
        }

        return false;
    }

    private static bool IsAccessible(SyntaxKind syntaxKind)
    {
        return syntaxKind == SyntaxKind.PublicKeyword || syntaxKind == SyntaxKind.InternalKeyword;
    }
}