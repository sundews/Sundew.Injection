// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeFactory.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

public sealed class TypeFactory
{
    private readonly IKnownInjectableTypes knownInjectableTypes;

    public TypeFactory(IKnownInjectableTypes knownInjectableTypes)
    {
        this.knownInjectableTypes = knownInjectableTypes;
    }

    public (Type Type, TypeMetadata TypeMetadata) CreateType(ITypeSymbol typeSymbol)
    {
        var fullResolvableType = TypeConverter.GetType(typeSymbol, this.knownInjectableTypes);
        return (fullResolvableType.Type, this.GetTypeMetadata(typeSymbol, TypeConverter.GetConstructor(fullResolvableType.Constructors.GetMethodWithMostParameters(), fullResolvableType.Type, this.knownInjectableTypes)));
    }

    public NamedType CreateNamedType(INamedTypeSymbol namedTypeSymbol)
    {
        return TypeConverter.GetNamedType(namedTypeSymbol);
    }

    public Method CreateMethod(IPropertySymbol propertySymbol)
    {
        return TypeConverter.GetMethod(propertySymbol, this.knownInjectableTypes);
    }

    public Method CreateMethod(IMethodSymbol methodSymbol)
    {
        return TypeConverter.GetMethod(methodSymbol, this.knownInjectableTypes);
    }

    public (GenericType Type, TypeMetadata TypeMatadata) GetGenericType(INamedTypeSymbol namedTypeSymbol)
    {
        var genericTypeSymbol = namedTypeSymbol.ConstructedFrom;
        return (GenericTypeConverter.GetGenericType(genericTypeSymbol), this.GetTypeMetadata(genericTypeSymbol, default));
    }

    public GenericMethod GetGenericMethod(IMethodSymbol? methodSymbol)
    {
        return methodSymbol != null ? new GenericMethod(methodSymbol.Parameters.Select(this.GetGenericParameter).ToImmutableArray(), methodSymbol.MetadataName, TypeConverter.GetContaineeType(methodSymbol), methodSymbol.MethodKind == MethodKind.Constructor) : default;
    }

    public GenericParameter GetGenericParameter(IParameterSymbol parameterSymbol)
    {
        var fullSymbol = TypeConverter.GetSymbol(parameterSymbol.Type, this.knownInjectableTypes);
        return new GenericParameter(fullSymbol.Symbol, parameterSymbol.MetadataName, this.GetTypeMetadata(parameterSymbol.Type, TypeConverter.GetConstructor(fullSymbol.Constructors.GetMethodWithMostParameters(), fullSymbol.Symbol as Type, this.knownInjectableTypes)));
    }

    public (UnboundGenericType Type, TypeMetadata TypeMetadata) GetUnboundGenericType(INamedTypeSymbol unboundGenericTypeSymbol)
    {
        return (GenericTypeConverter.GetUnboundGenericType(unboundGenericTypeSymbol), this.GetTypeMetadata(unboundGenericTypeSymbol, default));
    }

    private TypeMetadata GetTypeMetadata(ITypeSymbol typeSymbol, Method? defaultConstructor)
    {
    return TypeConverter.GetTypeMetadata(typeSymbol, defaultConstructor, this.knownInjectableTypes);
    }
}