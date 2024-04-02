// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeFactory.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sundew.Base;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

internal sealed class TypeFactory(
    IKnownInjectableTypes knownInjectableTypes)
{
    public R<NamedType, MappedTypeSymbol> GetNamedType(MappedTypeSymbol typeSymbol)
    {
        return TypeConverter.GetNamedType(typeSymbol);
    }

    public (Type Type, TypeMetadata TypeMetadata) CreateType(ITypeSymbol typeSymbol)
    {
        var fullResolvableType = TypeConverter.GetType(typeSymbol, knownInjectableTypes);
        return (fullResolvableType.Type, this.GetTypeMetadata(typeSymbol, TypeConverter.GetConstructor(fullResolvableType.Constructors.GetDefaultMethodWithMostParameters(), fullResolvableType.Type, knownInjectableTypes)));
    }

    public NamedType CreateNamedType(INamedTypeSymbol namedTypeSymbol)
    {
        return TypeConverter.GetNamedType(namedTypeSymbol);
    }

    public Method CreateMethod(IPropertySymbol propertySymbol)
    {
        return TypeConverter.GetMethod(propertySymbol, knownInjectableTypes);
    }

    public Method CreateMethod(IMethodSymbol methodSymbol)
    {
        return TypeConverter.GetMethod(methodSymbol, knownInjectableTypes);
    }

    public FactoryMethod CreateFactoryMethod(IMethodSymbol methodSymbol)
    {
        return TypeConverter.GetFactoryMethod(methodSymbol, knownInjectableTypes);
    }

    public (OpenGenericType Type, TypeMetadata TypeMatadata) GetGenericType(INamedTypeSymbol namedTypeSymbol)
    {
        var genericTypeSymbol = namedTypeSymbol.ConstructedFrom;
        return (GenericTypeConverter.GetGenericType(genericTypeSymbol), this.GetTypeMetadata(genericTypeSymbol, default));
    }

    public GenericMethod GetGenericMethod(IMethodSymbol? methodSymbol)
    {
        return methodSymbol != null ? new GenericMethod(methodSymbol.Parameters.Select(this.GetGenericParameter).ToImmutableArray(), methodSymbol.MetadataName, TypeConverter.GetContaineeType(methodSymbol), TypeConverter.GetMethodKind(methodSymbol, knownInjectableTypes)) : default;
    }

    public GenericParameter GetGenericParameter(IParameterSymbol parameterSymbol)
    {
        var fullSymbol = TypeConverter.GetSymbol(parameterSymbol.Type, knownInjectableTypes);
        return new GenericParameter(fullSymbol.Symbol, parameterSymbol.MetadataName, this.GetTypeMetadata(parameterSymbol.Type, TypeConverter.GetConstructor(fullSymbol.Constructors.GetDefaultMethodWithMostParameters(), fullSymbol.Symbol as Type, knownInjectableTypes)));
    }

    public (UnboundGenericType Type, TypeMetadata TypeMetadata) GetUnboundGenericType(INamedTypeSymbol unboundGenericTypeSymbol)
    {
        return (GenericTypeConverter.GetUnboundGenericType(unboundGenericTypeSymbol), this.GetTypeMetadata(unboundGenericTypeSymbol, default));
    }

    private TypeMetadata GetTypeMetadata(ITypeSymbol typeSymbol, Method? defaultConstructor)
    {
        return TypeConverter.GetTypeMetadata(typeSymbol, defaultConstructor, knownInjectableTypes);
    }
}