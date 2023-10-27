// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodFactory.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.TypeSystem;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Sundew.Base.Collections;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Primitives.Computation;
using Sundew.Base.Text;
using Sundew.Injection.Generator.TypeSystem;

internal sealed class MethodFactory
{
    private readonly TypeResolver typeResolver;

    public MethodFactory(TypeResolver typeResolver)
    {
        this.typeResolver = typeResolver;
    }

    public R<DefiniteMethod, CreateGenericMethodError> CreateMethod(DefiniteClosedGenericType definiteClosedGenericType, OpenGenericType openGenericType, GenericMethod genericMethod)
    {
        static Item<DefiniteParameter, Symbol> TryGetDefiniteArray(ArrayType arrayType, TypeMetadata typeMetadata, string parameterName, TypeResolver typeResolver)
        {
            var resolveTypeResult = typeResolver.ResolveType(arrayType.ElementType);
            if (resolveTypeResult.IsSuccess)
            {
                return Item.Pass(new DefiniteParameter(new DefiniteArrayType(resolveTypeResult.Value), parameterName, typeMetadata, ParameterNecessity._Required));
            }

            return Item.Fail<DefiniteParameter, Symbol>(arrayType);
        }

        static Item<DefiniteParameter, Symbol> TryGetDefiniteBoundGenericType(ClosedGenericType closedGenericType, TypeMetadata typeMetadata, string parameterName, TypeResolver typeResolver)
        {
            var resolveTypeResult = typeResolver.ResolveType(closedGenericType);
            return resolveTypeResult.IsSuccess
                ? Item.Pass(new DefiniteParameter(resolveTypeResult.Value, parameterName, typeMetadata, ParameterNecessity._Required))
                : Item.Fail<DefiniteParameter, Symbol>(closedGenericType);
        }

        static Item<DefiniteParameter, Symbol> LookupOpenGenericType(OpenGenericType openGenericType, string parameterName, TypeMetadata typeMetadata, Dictionary<TypeParameter, DefiniteTypeArgument> typeParameterDictionary)
        {
            var lookupResult = openGenericType.TypeParameters.AllOrFailed(x => LookupTypeParameter(x, x.Name, typeParameterDictionary));
            if (lookupResult.IsSuccess)
            {
                return Item.Pass(new DefiniteParameter(openGenericType.ToDefiniteClosedGenericType(lookupResult.Value.Items.Select(x => new DefiniteTypeArgument(x.Type, x.TypeMetadata)).ToValueArray()), parameterName, typeMetadata, ParameterNecessity._Required));
            }

            return Item.Fail<DefiniteParameter, Symbol>(openGenericType);
        }

        static Item<DefiniteParameter, Symbol> LookupTypeParameter(TypeParameter typeParameter, string parameterName, Dictionary<TypeParameter, DefiniteTypeArgument> typeParameterDictionary)
        {
            return typeParameterDictionary.TryGetValue(typeParameter, out var definiteTypeArgument)
                ? Item.Pass(new DefiniteParameter(definiteTypeArgument.Type, parameterName, definiteTypeArgument.TypeMetadata, ParameterNecessity._Required))
                : Item.Fail<DefiniteParameter, Symbol>(typeParameter);
        }

        static Item<DefiniteParameter, Symbol> LookupTypeParameterArray(TypeParameterArray typeParameterArray, string parameterName, Dictionary<TypeParameter, DefiniteTypeArgument> typeParameterDictionary, TypeResolver typeResolver)
        {
            if (typeParameterDictionary.TryGetValue(typeParameterArray.ElementTypeParameter, out var definiteTypeArgument))
            {
                var resolveTypeResult = typeResolver.ResolveType(definiteTypeArgument.Type);
                return resolveTypeResult.IsSuccess
                    ? Item.Pass(new DefiniteParameter(new DefiniteArrayType(resolveTypeResult.Value), parameterName, new TypeMetadata(null, new EnumerableMetadata(true, true, true), false), ParameterNecessity._Required))
                    : Item.Fail<DefiniteParameter, Symbol>(typeParameterArray);
            }

            return Item.Fail<DefiniteParameter, Symbol>(typeParameterArray);
        }

        static Item<DefiniteParameter, Symbol> TryGetDefiniteNestedType(NestedType nestedType, TypeMetadata typeMetadata, string parameterName, TypeResolver typeResolver)
        {
            var resolveContainingTypeResult = typeResolver.ResolveType(nestedType.ContainingType);
            var resolveContainedTypeResult = typeResolver.ResolveType(nestedType.ContainedType);
            if (resolveContainingTypeResult.IsSuccess && resolveContainedTypeResult.IsSuccess)
            {
                return Item.Pass(new DefiniteParameter(new DefiniteNestedType(resolveContainedTypeResult.Value, resolveContainingTypeResult.Value), parameterName, typeMetadata, ParameterNecessity._Required));
            }

            return Item.Fail<DefiniteParameter, Symbol>(nestedType);
        }

        var typeParameterDictionary = openGenericType.TypeParameters
            .Zip(definiteClosedGenericType.TypeArguments, (symbol, typeArgument) => (symbol, typeArgument))
            .ToDictionary(tuple => tuple.symbol, tuple => tuple.typeArgument);
        var methodParameters = genericMethod.Parameters.AllOrFailed(
            x =>
            {
                return x.Type switch
                {
                    ArrayType arrayType => TryGetDefiniteArray(arrayType, x.TypeMetadata, x.Name, this.typeResolver),
                    NamedType namedType => Item.Pass(new DefiniteParameter(namedType, x.Name, x.TypeMetadata, ParameterNecessity._Required)),
                    NestedType nestedType => TryGetDefiniteNestedType(nestedType, x.TypeMetadata, x.Name, this.typeResolver),
                    OpenGenericType openGenericType => LookupOpenGenericType(openGenericType, x.Name, x.TypeMetadata, typeParameterDictionary),
                    ClosedGenericType boundGenericType => TryGetDefiniteBoundGenericType(boundGenericType, x.TypeMetadata, x.Name, this.typeResolver),
                    ErrorType errorType => Item.Fail<DefiniteParameter, Symbol>(errorType),
                    TypeParameter typeParameter => LookupTypeParameter(typeParameter, x.Name, typeParameterDictionary),
                    TypeParameterArray typeParameterArray => LookupTypeParameterArray(typeParameterArray, x.Name, typeParameterDictionary, this.typeResolver),
                    DefiniteClosedGenericType definiteBoundGenericType => Item.Pass(new DefiniteParameter(definiteBoundGenericType, x.Name, x.TypeMetadata, ParameterNecessity._Required)),
                    DefiniteNestedType definiteNestedType => Item.Pass(new DefiniteParameter(definiteNestedType, x.Name, x.TypeMetadata, ParameterNecessity._Required)),
                    DefiniteArrayType definiteArrayType => Item.Pass(new DefiniteParameter(definiteArrayType, x.Name, x.TypeMetadata, ParameterNecessity._Required)),
                };
            });

        return methodParameters.With(
            all => CreateMethod(genericMethod, all.Items.ToImmutableArray(), definiteClosedGenericType),
            failed => new CreateGenericMethodError(failed.Items.Select(x => (x.Item, x.Error)).ToImmutableArray()));
    }

    public R<DefiniteMethod, CreateMethodError> CreateMethod(Method method, string? overrideMethodName = null)
    {
        var result = this.typeResolver.ResolveType(method.ContainingType);
        if (result.IsSuccess)
        {
            var allOrFailed = method.Parameters.AllOrFailed(x =>
            {
                var resolveTypeResult = this.typeResolver.ResolveType(x.Type);
                return resolveTypeResult.IsSuccess
                    ? Item.Pass(new DefiniteParameter(resolveTypeResult.Value, x.Name, x.TypeMetadata, x.ParameterNecessity))
                    : Item.Fail();
            });

            return allOrFailed.With(
                all => new DefiniteMethod(result.Value, overrideMethodName.IsNullOrEmpty() ? method.Name : overrideMethodName, all.Items.ToImmutableArray(), ImmutableArray<DefiniteTypeArgument>.Empty, method.Kind),
                failedItems => new CreateMethodError(default, failedItems.Items.Select(x => x.Item).ToImmutableArray()));
        }

        return R.Error(new CreateMethodError(method.ContainingType, default));
    }

    private static DefiniteMethod CreateMethod(GenericMethod genericMethod, ValueArray<DefiniteParameter> parameters, DefiniteClosedGenericType definiteClosedGenericType)
    {
        return new DefiniteMethod(GetDefiniteType(genericMethod.ContainedType, definiteClosedGenericType.TypeArguments), genericMethod.Name, parameters, definiteClosedGenericType.TypeArguments, genericMethod.Kind);
    }

    private static DefiniteType GetDefiniteType(ContaineeType containeeType, ValueArray<DefiniteTypeArgument> typeArguments)
    {
        return containeeType switch
        {
            ContaineeType.GenericType genericType => genericType.ToDefiniteClosedGenericType(typeArguments),
            ContaineeType.NamedType namedType => DefiniteType.NamedType(namedType.Name, namedType.Namespace, namedType.AssemblyName, namedType.IsValueType),
        };
    }
}