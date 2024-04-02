// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeConverter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

internal static class TypeConverter
{
    public static SymbolDisplayFormat NameQualifiedTypeFormat { get; } =
        new(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            localOptions: SymbolDisplayLocalOptions.IncludeType,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers | SymbolDisplayMiscellaneousOptions.UseSpecialTypes | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

    public static (Symbol Symbol, ImmutableArray<IMethodSymbol> Constructors) GetSymbol(ITypeSymbol typeSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        return typeSymbol switch
        {
            IErrorTypeSymbol errorTypeSymbol => (new ErrorType(errorTypeSymbol.MetadataName), ImmutableArray<IMethodSymbol>.Empty),
            IArrayTypeSymbol arrayTypeSymbol => (GetArrayOrTypeParameterArray(arrayTypeSymbol, knownInjectableTypes), ImmutableArray<IMethodSymbol>.Empty),
            INamedTypeSymbol namedTypeSymbol => (GetNamedSymbol(namedTypeSymbol, knownInjectableTypes), namedTypeSymbol.Constructors),
            ITypeParameterSymbol typeParameterSymbol => (new TypeParameter(typeParameterSymbol.MetadataName), ImmutableArray<IMethodSymbol>.Empty),
            _ => throw new System.NotSupportedException($"The type {typeSymbol} is currently not supported."),
        };
    }

    public static Symbol GetArrayOrTypeParameterArray(IArrayTypeSymbol arrayTypeSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        if (arrayTypeSymbol.ElementType is ITypeParameterSymbol typeParameterSymbol)
        {
            return Symbol.TypeParameterArray(new TypeParameter(typeParameterSymbol.MetadataName));
        }

        return GetArrayType(arrayTypeSymbol, knownInjectableTypes);
    }

    public static (Type Type, ImmutableArray<IMethodSymbol> Constructors) GetType(ITypeSymbol typeSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        return typeSymbol switch
        {
            IErrorTypeSymbol errorTypeSymbol => (new ErrorType(errorTypeSymbol.MetadataName), default),
            IArrayTypeSymbol arrayTypeSymbol => (GetArrayType(arrayTypeSymbol, knownInjectableTypes), default),
            INamedTypeSymbol namedTypeSymbol => (GetNamedOrClosedGenericType(namedTypeSymbol, knownInjectableTypes), namedTypeSymbol.Constructors),
            _ => throw new System.NotSupportedException($"The type {typeSymbol} is currently not supported."),
        };
    }

    public static R<NamedType, MappedTypeSymbol> GetNamedType(MappedTypeSymbol typeSymbol)
    {
        return typeSymbol.TypeSymbol switch
        {
            INamedTypeSymbol namedTypeSymbol => R.Success(GetNamedType(namedTypeSymbol)),
            _ => R.Error(typeSymbol),
        };
    }

    public static ArrayType GetArrayType(IArrayTypeSymbol arrayTypeSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        var type = GetType(arrayTypeSymbol.ElementType, knownInjectableTypes);
        return new ArrayType(type.Type);
    }

    public static NamedType GetNamedType(INamedTypeSymbol namedTypeSymbol)
    {
        var (name, @namespace, _) = GetName(namedTypeSymbol);
        return new NamedType(
            name,
            @namespace,
            namedTypeSymbol.ContainingAssembly.Identity.ToString(),
            namedTypeSymbol.IsValueType);
    }

    public static Method? GetConstructor(IMethodSymbol? methodSymbol, Type? containingType, IKnownInjectableTypes knownInjectableTypes)
    {
        if (methodSymbol != null && containingType != null && methodSymbol.ContainingType.IsInstantiable())
        {
            return new Method(
                methodSymbol.Parameters.Select(x => GetParameter(x, knownInjectableTypes)).ToImmutableArray(),
                methodSymbol.MetadataName,
                containingType,
                MethodKind._Constructor);
        }

        return default;
    }

    [return: NotNullIfNotNull(nameof(methodSymbol))]
    public static Method? GetMethod(IMethodSymbol? methodSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        if (methodSymbol != null)
        {
            return new Method(
                methodSymbol.Parameters.Select(x => GetParameter(x, knownInjectableTypes)).ToImmutableArray(),
                methodSymbol.MetadataName,
                GetType(methodSymbol.ContainingType, knownInjectableTypes).Type,
                TypeConverter.GetMethodKind(methodSymbol, knownInjectableTypes));
        }

        return default;
    }

    [return: NotNullIfNotNull(nameof(methodSymbol))]
    public static FactoryMethod GetFactoryMethod(IMethodSymbol? methodSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        if (methodSymbol != null)
        {
            return new FactoryMethod(
                methodSymbol.MetadataName,
                methodSymbol.Parameters.Select(x => GetParameter(x, knownInjectableTypes)).ToImmutableArray(),
                GetType(methodSymbol.ReturnType, knownInjectableTypes).Type);
        }

        return default;
    }

    [return: NotNullIfNotNull(nameof(propertySymbol))]
    public static Method? GetMethod(IPropertySymbol? propertySymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        if (propertySymbol != null && propertySymbol.GetMethod != null)
        {
            return new Method(
                propertySymbol.Parameters.Select(x => GetParameter(x, knownInjectableTypes)).ToImmutableArray(),
                propertySymbol.MetadataName,
                GetType(propertySymbol.ContainingType, knownInjectableTypes).Type,
                GetMethodKind(propertySymbol.GetMethod, knownInjectableTypes));
        }

        return default;
    }

    public static Parameter GetParameter(IParameterSymbol parameterSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        var fullType = GetType(parameterSymbol.Type, knownInjectableTypes);
        return new Parameter(fullType.Type, parameterSymbol.MetadataName, GetTypeMetadata(parameterSymbol.Type, GetConstructor(fullType.Constructors.GetDefaultMethodWithMostParameters(), fullType.Type, knownInjectableTypes), knownInjectableTypes), GetParameterNecessity(parameterSymbol));
    }

    public static ParameterNecessity GetParameterNecessity(IParameterSymbol parameterSymbol)
    {
        if (parameterSymbol.HasExplicitDefaultValue)
        {
            var defaultValue = parameterSymbol.ExplicitDefaultValue switch
            {
                bool boolean => boolean.ToString().ToLowerInvariant(),
                not null => parameterSymbol.ExplicitDefaultValue,
                null => null,
            };

            return new ParameterNecessity.Optional(defaultValue);
        }

        if (parameterSymbol.NullableAnnotation == NullableAnnotation.Annotated)
        {
            return new ParameterNecessity.Optional(null);
        }

        return ParameterNecessity._Required;
    }

    public static ContaineeType GetContaineeType(IMethodSymbol methodSymbol)
    {
        var containingType = methodSymbol.ContainingType;
        if (containingType.IsGenericType && !containingType.IsUnboundGenericType)
        {
            return new ContaineeType.GenericType(
                containingType.MetadataName,
                TypeHelper.GetNamespace(containingType.ContainingNamespace),
                containingType.ContainingAssembly.Identity.ToString(),
                containingType.TypeParameters.Select(x => new TypeParameter(x.MetadataName)).ToImmutableArray(),
                containingType.IsValueType);
        }

        return new ContaineeType.NamedType(containingType.MetadataName, TypeHelper.GetNamespace(containingType.ContainingNamespace), containingType.ContainingAssembly.Identity.ToString(), containingType.IsValueType);
    }

    public static TypeMetadata GetTypeMetadata(ITypeSymbol typeSymbol, Method? defaultConstructor, IKnownInjectableTypes knownInjectableTypes)
    {
        var implementIEnumerable = typeSymbol is IArrayTypeSymbol || typeSymbol.OriginalDefinition.CanBeAssignedTo(knownInjectableTypes.IEnumerableOfTTypeSymbol);
        var enumerableMetadata = implementIEnumerable
            ? new EnumerableMetadata(true, GetArrayMetadata(typeSymbol, knownInjectableTypes))
            : new EnumerableMetadata(false, false, false);
        return new TypeMetadata(defaultConstructor, enumerableMetadata, HasLifecycle(typeSymbol, knownInjectableTypes));
    }

    public static MethodKind GetMethodKind(IMethodSymbol methodSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        return methodSymbol switch
        {
            { IsStatic: true } => MethodKind._Static,
            { MethodKind: Microsoft.CodeAnalysis.MethodKind.Constructor } => MethodKind._Constructor,
            { IsStatic: false } => MethodKind._Instance(GetTypeMetadata(methodSymbol.ContainingType, null, knownInjectableTypes), false),
            _ => throw new NotSupportedException($"The method is not supported: {methodSymbol}"),
        };
    }

    private static bool HasLifecycle(ITypeSymbol typeSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        return typeSymbol.CanBeAssignedTo(knownInjectableTypes.IAsyncDisposableTypeSymbol) ||
               typeSymbol.CanBeAssignedTo(knownInjectableTypes.IDisposableTypeSymbol) ||
               typeSymbol.CanBeAssignedTo(knownInjectableTypes.IAsyncInitializableTypeSymbol) ||
               typeSymbol.CanBeAssignedTo(knownInjectableTypes.IInitializableTypeSymbol);
    }

    private static (bool IsArrayCompatible, bool IsArrayRequired) GetArrayMetadata(ITypeSymbol typeSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        var originalTypeSymbol = typeSymbol.OriginalDefinition;
        var isIEnumerable = SymbolEqualityComparer.Default.Equals(originalTypeSymbol, knownInjectableTypes.IEnumerableOfTTypeSymbol);
        var isReadOnlyList = SymbolEqualityComparer.Default.Equals(originalTypeSymbol, knownInjectableTypes.IReadOnlyListOfTTypeSymbol);
        return (isReadOnlyList || isIEnumerable, !isIEnumerable);
    }

    private static Symbol GetNamedSymbol(INamedTypeSymbol namedTypeSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        if (namedTypeSymbol.TypeArguments.Any(x => x is ITypeParameterSymbol))
        {
            var (name, @namespace, _) = GetName(namedTypeSymbol);
            return Symbol.OpenGenericType(
                name,
                @namespace,
                namedTypeSymbol.ContainingAssembly.Identity.ToString(),
                namedTypeSymbol.TypeParameters.Select(x => new TypeParameter(x.MetadataName)).ToValueArray(),
                namedTypeSymbol.IsValueType);
        }

        return GetNamedOrClosedGenericType(namedTypeSymbol, knownInjectableTypes);
    }

    private static Type GetNamedOrClosedGenericType(INamedTypeSymbol namedTypeSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        if (namedTypeSymbol.IsGenericType && !namedTypeSymbol.IsUnboundGenericType)
        {
            var (name, @namespace, _) = GetName(namedTypeSymbol);
            return Type.ClosedGenericType(
                name,
                @namespace,
                namedTypeSymbol.ContainingAssembly.Identity.ToString(),
                namedTypeSymbol.TypeParameters.Select(x => new TypeParameter(x.MetadataName)).ToImmutableArray(),
                namedTypeSymbol.TypeArguments.Select(x => GetTypeArgument(x, knownInjectableTypes)).ToImmutableArray(),
                namedTypeSymbol.IsValueType);
        }

        return GetNamedType(namedTypeSymbol);
    }

    private static TypeArgument GetTypeArgument(ITypeSymbol typeSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        var fullType = GetType(typeSymbol, knownInjectableTypes);
        return new TypeArgument(fullType.Type, GetTypeMetadata(typeSymbol, GetConstructor(fullType.Constructors.GetDefaultMethodWithMostParameters(), fullType.Type, knownInjectableTypes), knownInjectableTypes));
    }

    private static (string Name, string Namespace, bool IsShortNameAlias) GetName(ITypeSymbol typeSymbol)
    {
        switch (typeSymbol.SpecialType)
        {
            case SpecialType.System_Object:
            case SpecialType.System_Boolean:
            case SpecialType.System_Char:
            case SpecialType.System_SByte:
            case SpecialType.System_Byte:
            case SpecialType.System_Int16:
            case SpecialType.System_UInt16:
            case SpecialType.System_Int32:
            case SpecialType.System_UInt32:
            case SpecialType.System_Int64:
            case SpecialType.System_UInt64:
            case SpecialType.System_Decimal:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            case SpecialType.System_String:
                return (typeSymbol.ToDisplayString(NameQualifiedTypeFormat), string.Empty, true);
            default:
                if (typeSymbol.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
                {
                    return (typeSymbol.ToDisplayString(NameQualifiedTypeFormat), string.Empty, true);
                }

                var format = new SymbolDisplayFormat(
                    globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
                    typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
                    genericsOptions: SymbolDisplayGenericsOptions.None,
                    miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers | SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

                var name = typeSymbol.ToDisplayString(format);
                return (name, TypeHelper.GetNamespace(typeSymbol.ContainingNamespace), false);
        }
    }
}