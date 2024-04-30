// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeConverter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Collections.Linq;
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

    public static SymbolDisplayFormat TypeNameFormat { get; } = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
        genericsOptions: SymbolDisplayGenericsOptions.None,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers | SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

    public static (Symbol Symbol, ImmutableArray<IMethodSymbol> Constructors) GetSymbolWithConstructors(ITypeSymbol typeSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        return typeSymbol switch
        {
            IArrayTypeSymbol arrayTypeSymbol => (GetArrayOrTypeParameterArray(arrayTypeSymbol, knownInjectableTypes), ImmutableArray<IMethodSymbol>.Empty),
            INamedTypeSymbol namedTypeSymbol => (GetNamedSymbol(namedTypeSymbol, knownInjectableTypes), namedTypeSymbol.Constructors),
            ITypeParameterSymbol typeParameterSymbol => (new TypeParameter(typeParameterSymbol.MetadataName), ImmutableArray<IMethodSymbol>.Empty),
            _ => throw new System.NotSupportedException($"The type {typeSymbol} is currently not supported."),
        };
    }

    public static Type GetType(ITypeSymbol typeSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        return GetTypeWithConstructors(typeSymbol, knownInjectableTypes).Type;
    }

    public static (Type Type, ImmutableArray<IMethodSymbol> Constructors) GetTypeWithConstructors(ITypeSymbol typeSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        return typeSymbol switch
        {
            IArrayTypeSymbol arrayTypeSymbol => (GetArrayType(arrayTypeSymbol, knownInjectableTypes), default),
            INamedTypeSymbol namedTypeSymbol => (GetNamedOrClosedGenericType(namedTypeSymbol, knownInjectableTypes), namedTypeSymbol.Constructors),
            _ => throw new System.NotSupportedException($"The type {typeSymbol} is currently not supported."),
        };
    }

    public static R<NamedType, TypeSymbolWithLocation> GetNamedType(TypeSymbolWithLocation typeSymbolWithLocation)
    {
        return typeSymbolWithLocation.TypeSymbol switch
        {
            INamedTypeSymbol namedTypeSymbol => R.Success(GetNamedType(namedTypeSymbol)),
            _ => R.Error(typeSymbolWithLocation),
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

    public static ArrayType GetArrayType(IArrayTypeSymbol arrayTypeSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        var type = GetType(arrayTypeSymbol.ElementType, knownInjectableTypes);
        return new ArrayType(type);
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

    public static R<Method?, SymbolError> GetConstructor(IMethodSymbol? methodSymbol, Type? containingType, IKnownInjectableTypes knownInjectableTypes, ImmutableHashSet<TypeId> visitedTypes)
    {
        if (methodSymbol != null && containingType != null && methodSymbol.ContainingType.IsInstantiable())
        {
            (visitedTypes, var wasAdded) = visitedTypes.TryAdd(containingType.Id);
            if (!wasAdded)
            {
                return R.Error(new SymbolError(containingType, ImmutableList<SymbolError>.Empty));
            }

            var parametersResult = methodSymbol.Parameters.AllOrFailed(x => GetFullParameter(x, knownInjectableTypes, visitedTypes).ToItem());
            if (parametersResult.TryGet(out var all, out var errors))
            {
                return R.SuccessOption(new Method(
                    containingType,
                    methodSymbol.MetadataName,
                    all.Items,
                    ValueArray<FullTypeArgument>.Empty,
                    MethodKind._Constructor));
            }

            return R.Error(new SymbolError(containingType, errors.GetErrors()));
        }

        return R.SuccessOption<Method?>();
    }

    public static R<Method, SymbolError> GetMethod(IMethodSymbol methodSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        var containingType = GetType(methodSymbol.ContainingType, knownInjectableTypes);
        var parameters = methodSymbol.Parameters.AllOrFailed(x => GetFullParameter(x, knownInjectableTypes, ImmutableHashSet<TypeId>.Empty).ToItem());
        if (parameters.IsError)
        {
            return R.Error(new SymbolError(containingType, parameters.Error.GetErrors()));
        }

        return R.Success(
            new Method(
                containingType,
                methodSymbol.MetadataName,
                parameters.Value.Items,
                ValueArray<FullTypeArgument>.Empty,
                GetMethodKind(methodSymbol, knownInjectableTypes)));
    }

    public static Method? GetMethod(IPropertySymbol propertySymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        if (propertySymbol.GetMethod != null)
        {
            return new Method(
                GetType(propertySymbol.ContainingType, knownInjectableTypes),
                propertySymbol.MetadataName,
                ValueArray<FullParameter>.Empty,
                ValueArray<FullTypeArgument>.Empty,
                GetMethodKind(propertySymbol.GetMethod, knownInjectableTypes));
        }

        return default;
    }

    public static FactoryTarget GetFactoryTarget(IMethodSymbol? methodSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        if (methodSymbol != null)
        {
            return new FactoryTarget(
                methodSymbol.MetadataName,
                methodSymbol.Parameters.Select(x => GetParameter(x, knownInjectableTypes)).ToValueList(),
                GetType(methodSymbol.ReturnType, knownInjectableTypes),
                false);
        }

        return default;
    }

    public static FactoryTarget GetFactoryTarget(IPropertySymbol? propertySymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        if (propertySymbol != null)
        {
            return new FactoryTarget(
                propertySymbol.MetadataName,
                ValueList<Parameter>.Empty,
                GetType(propertySymbol.Type, knownInjectableTypes),
                true);
        }

        return default;
    }

    public static Parameter GetParameter(IParameterSymbol parameterSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        return new Parameter(GetType(parameterSymbol.Type, knownInjectableTypes), parameterSymbol.MetadataName);
    }

    public static R<FullParameter, SymbolError> GetFullParameter(IParameterSymbol parameterSymbol, IKnownInjectableTypes knownInjectableTypes, ImmutableHashSet<TypeId> visitedTypes)
    {
        var typeWithConstructors = GetTypeWithConstructors(parameterSymbol.Type, knownInjectableTypes);
        var constructorResult = GetConstructor(typeWithConstructors.Constructors.GetDefaultMethodWithMostParameters(), typeWithConstructors.Type, knownInjectableTypes, visitedTypes);
        return constructorResult.With(
            constructor => new FullParameter(typeWithConstructors.Type, parameterSymbol.MetadataName, GetTypeMetadata(parameterSymbol.Type, knownInjectableTypes), constructor, GetParameterNecessity(parameterSymbol)));
    }

    public static ParameterNecessity GetParameterNecessity(IParameterSymbol parameterSymbol)
    {
        if (parameterSymbol.HasExplicitDefaultValue)
        {
            var defaultValue = parameterSymbol.ExplicitDefaultValue switch
            {
                bool boolean => boolean.ToString().ToLowerInvariant(),
                not null => parameterSymbol.ExplicitDefaultValue,
                null => default,
            };

            return new ParameterNecessity.Optional(defaultValue);
        }

        return parameterSymbol.NullableAnnotation == NullableAnnotation.Annotated ? new ParameterNecessity.Optional(default) : ParameterNecessity._Required;
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

    public static TypeMetadata GetTypeMetadata(ITypeSymbol typeSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        var implementIEnumerable = typeSymbol is IArrayTypeSymbol || typeSymbol.OriginalDefinition.CanBeAssignedTo(knownInjectableTypes.IEnumerableOfTTypeSymbol);
        var enumerableMetadata = implementIEnumerable
            ? new EnumerableMetadata(true, GetArrayMetadata(typeSymbol, knownInjectableTypes))
            : new EnumerableMetadata(false, false, false);
        return new TypeMetadata(enumerableMetadata, HasLifecycle(typeSymbol, knownInjectableTypes));
    }

    public static MethodKind GetMethodKind(IMethodSymbol methodSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        return methodSymbol switch
        {
            { IsStatic: true } => MethodKind._Static,
            { MethodKind: Microsoft.CodeAnalysis.MethodKind.Constructor } => MethodKind._Constructor,
            { IsStatic: false } => MethodKind._Instance(GetTypeMetadata(methodSymbol.ContainingType, knownInjectableTypes), methodSymbol.MethodKind == Microsoft.CodeAnalysis.MethodKind.PropertyGet, default),
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
            return R.Success(
                Symbol.OpenGenericType(
                name,
                @namespace,
                namedTypeSymbol.ContainingAssembly.Identity.ToString(),
                namedTypeSymbol.TypeParameters.Select(x => new TypeParameter(x.MetadataName)).ToValueArray(),
                namedTypeSymbol.IsValueType));
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

    private static FullTypeArgument GetTypeArgument(ITypeSymbol typeSymbol, IKnownInjectableTypes knownInjectableTypes)
    {
        var type = GetType(typeSymbol, knownInjectableTypes);
        return new FullTypeArgument(type, GetTypeMetadata(typeSymbol, knownInjectableTypes));
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

                var name = typeSymbol.ToDisplayString(TypeNameFormat);
                return (name, TypeHelper.GetNamespace(typeSymbol.ContainingNamespace), false);
        }
    }
}