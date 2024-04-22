// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeFactory.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Sundew.Base;
using Sundew.Base.Collections.Linq;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

internal sealed class TypeFactory(
    IKnownInjectableTypes knownInjectableTypes)
{
    public R<NamedType, TypeSymbolWithLocation> GetNamedType(TypeSymbolWithLocation typeSymbolWithLocation)
    {
        return TypeConverter.GetNamedType(typeSymbolWithLocation);
    }

    public Type GetType(ITypeSymbol typeSymbol)
    {
        return TypeConverter.GetType(typeSymbol, knownInjectableTypes);
    }

    public R<FullType, SymbolErrorWithLocation> GetFullType(TypeSymbolWithLocation typeSymbolWithLocation)
    {
        var constructorResult = this.GetFullType(typeSymbolWithLocation.TypeSymbol);
        return constructorResult.WithError(error => new SymbolErrorWithLocation(error, typeSymbolWithLocation.Location));
    }

    public R<FullType, SymbolError> GetFullType(ITypeSymbol typeSymbol)
    {
        var type = TypeConverter.GetTypeWithConstructors(typeSymbol, knownInjectableTypes);
        var constructorResult = TypeConverter.GetConstructor(type.Constructors.GetDefaultMethodWithMostParameters(), type.Type, knownInjectableTypes, ImmutableHashSet<TypeId>.Empty);
        return constructorResult.With(constructor => new FullType(type.Type, this.GetTypeMetadata(typeSymbol), constructor));
    }

    public NamedType GetNamedType(INamedTypeSymbol namedTypeSymbol)
    {
        return TypeConverter.GetNamedType(namedTypeSymbol);
    }

    public Method? GetFactoryMethod(IPropertySymbol propertySymbol)
    {
        return TypeConverter.GetMethod(propertySymbol, knownInjectableTypes);
    }

    public R<Method, SymbolError> GetFactoryMethod(IMethodSymbol methodSymbol)
    {
        return TypeConverter.GetMethod(methodSymbol, knownInjectableTypes);
    }

    public FactoryTarget GetFactoryTarget(IMethodSymbol methodSymbol)
    {
        return TypeConverter.GetFactoryTarget(methodSymbol, knownInjectableTypes);
    }

    public FactoryTarget GetFactoryTarget(IPropertySymbol propertySymbol)
    {
        return TypeConverter.GetFactoryTarget(propertySymbol, knownInjectableTypes);
    }

    public (OpenGenericType Type, TypeMetadata TypeMatadata) GetGenericType(INamedTypeSymbol namedTypeSymbol)
    {
        var genericTypeSymbol = namedTypeSymbol.ConstructedFrom;
        return (GenericTypeConverter.GetGenericType(genericTypeSymbol), this.GetTypeMetadata(genericTypeSymbol));
    }

    public R<GenericMethod, SymbolError> GetGenericMethod(IMethodSymbol? methodSymbol)
    {
        if (methodSymbol.HasValue())
        {
            var genericParametersResult = methodSymbol.Parameters.AllOrFailed(x => this.GetGenericParameter(x).ToItem());
            if (genericParametersResult.IsError)
            {
                return R.Error(new SymbolError(new NamedSymbol(methodSymbol.ToDisplayString()), genericParametersResult.Error.GetErrors()));
            }

            return R.Success(
                new GenericMethod(
                    methodSymbol.MetadataName,
                    genericParametersResult.Value.Items,
                    TypeConverter.GetContaineeType(methodSymbol),
                    TypeConverter.GetMethodKind(methodSymbol, knownInjectableTypes)));
        }

        return R.Success();
    }

    public R<GenericParameter, SymbolError> GetGenericParameter(IParameterSymbol parameterSymbol)
    {
        var fullSymbol = TypeConverter.GetSymbolWithConstructors(parameterSymbol.Type, knownInjectableTypes);
        var defaultConstructorResult = TypeConverter.GetConstructor(fullSymbol.Constructors.GetDefaultMethodWithMostParameters(), fullSymbol.Symbol as Type, knownInjectableTypes, ImmutableHashSet<TypeId>.Empty);
        return defaultConstructorResult.With(x => new GenericParameter(fullSymbol.Symbol, parameterSymbol.MetadataName, this.GetTypeMetadata(parameterSymbol.Type), x));
    }

    public (UnboundGenericType Type, TypeMetadata TypeMetadata) GetUnboundGenericType(INamedTypeSymbol unboundGenericTypeSymbol)
    {
        return (GenericTypeConverter.GetUnboundGenericType(unboundGenericTypeSymbol), this.GetTypeMetadata(unboundGenericTypeSymbol));
    }

    private TypeMetadata GetTypeMetadata(ITypeSymbol typeSymbol)
    {
        return TypeConverter.GetTypeMetadata(typeSymbol, knownInjectableTypes);
    }
}