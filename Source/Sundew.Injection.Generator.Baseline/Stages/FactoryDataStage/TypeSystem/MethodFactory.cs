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

public sealed class MethodFactory
{
    private readonly TypeResolver typeResolver;

    public MethodFactory(TypeResolver typeResolver)
    {
        this.typeResolver = typeResolver;
    }

    public R<DefiniteMethod, CreateGenericMethodError> CreateMethod(DefiniteBoundGenericType definiteBoundGenericType, GenericType genericType, GenericMethod genericMethod)
    {
        static Item<DefiniteParameter, Symbol> TryGetDefiniteArray(ArrayType arrayType, TypeMetadata typeMetadata, string parameterName, TypeResolver typeResolver)
        {
            var resolveTypeResult = typeResolver.ResolveType(arrayType.ElementType);
            if (resolveTypeResult.IsSuccess)
            {
                return Item.Pass(new DefiniteParameter(new DefiniteArrayType(resolveTypeResult.Value), parameterName, typeMetadata));
            }

            return Item.Fail<DefiniteParameter, Symbol>(arrayType);
        }

        static Item<DefiniteParameter, Symbol> TryGetDefiniteBoundGenericType(BoundGenericType boundGenericType, TypeMetadata typeMetadata, string parameterName, TypeResolver typeResolver)
        {
            var resolveTypeResult = typeResolver.ResolveType(boundGenericType);
            return resolveTypeResult.IsSuccess
                ? Item.Pass(new DefiniteParameter(resolveTypeResult.Value, parameterName, typeMetadata))
                : Item.Fail<DefiniteParameter, Symbol>(boundGenericType);
        }

        static Item<DefiniteParameter, Symbol> LookupTypeParameter(TypeParameter typeParameter, string parameterName, Dictionary<TypeParameter, DefiniteTypeArgument> typeParameterDictionary)
        {
            return typeParameterDictionary.TryGetValue(typeParameter, out var definiteTypeArgument)
                ? Item.Pass(new DefiniteParameter(definiteTypeArgument.Type, parameterName, definiteTypeArgument.TypeMetadata))
                : Item.Fail<DefiniteParameter, Symbol>(typeParameter);
        }

        static Item<DefiniteParameter, Symbol> LookupTypeParameterArray(TypeParameterArray typeParameterArray, string parameterName, Dictionary<TypeParameter, DefiniteTypeArgument> typeParameterDictionary, TypeResolver typeResolver)
        {
            if (typeParameterDictionary.TryGetValue(typeParameterArray.ElementTypeParameter, out var definiteTypeArgument))
            {
                var resolveTypeResult = typeResolver.ResolveType(definiteTypeArgument.Type);
                return resolveTypeResult.IsSuccess
                    ? Item.Pass(new DefiniteParameter(new DefiniteArrayType(resolveTypeResult.Value), parameterName, new TypeMetadata(null, false, true, false)))
                    : Item.Fail<DefiniteParameter, Symbol>(typeParameterArray);
            }

            return Item.Fail<DefiniteParameter, Symbol>(typeParameterArray);
        }

        var typeParameterDictionary = genericType.TypeParameters
            .Zip(definiteBoundGenericType.TypeArguments, (symbol, typeArgument) => (symbol, typeArgument))
            .ToDictionary(tuple => tuple.symbol, tuple => tuple.typeArgument);
        var methodParameters = genericMethod.Parameters.AllOrFailed(
            x =>
            {
                return x.Type switch
                {
                    ArrayType arrayType => TryGetDefiniteArray(arrayType, x.TypeMetadata, x.Name, this.typeResolver),
                    NamedType namedType => Item.Pass(new DefiniteParameter(namedType, x.Name, x.TypeMetadata)),
                    BoundGenericType boundGenericType => TryGetDefiniteBoundGenericType(boundGenericType, x.TypeMetadata, x.Name, this.typeResolver),
                    ErrorType errorType => Item.Fail<DefiniteParameter, Symbol>(errorType),
                    TypeParameter typeParameter => LookupTypeParameter(typeParameter, x.Name, typeParameterDictionary),
                    TypeParameterArray typeParameterArray => LookupTypeParameterArray(typeParameterArray, x.Name, typeParameterDictionary, this.typeResolver),
                    DefiniteBoundGenericType definiteBoundGenericType => Item.Pass(new DefiniteParameter(definiteBoundGenericType, x.Name, x.TypeMetadata)),
                    DefiniteArrayType definiteArrayType => Item.Pass(new DefiniteParameter(definiteArrayType, x.Name, x.TypeMetadata)),
                };
            });

        return methodParameters.With(
            all => CreateMethod(genericMethod, all.Items.ToImmutableArray(), definiteBoundGenericType),
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
                    ? Item.Pass(new DefiniteParameter(resolveTypeResult.Value, x.Name, x.TypeMetadata))
                    : Item.Fail();
            });

            return allOrFailed.With(
                all => new DefiniteMethod(all.Items.ToImmutableArray(), overrideMethodName.IsNullOrEmpty() ? method.Name : overrideMethodName, result.Value, ImmutableArray<DefiniteTypeArgument>.Empty, method.IsConstructor),
                failed => new CreateMethodError(default, failed.Items.Select(x => x.Item).ToImmutableArray()));
        }

        return R.Error(new CreateMethodError(method.ContainingType, default));
    }

    private static DefiniteMethod CreateMethod(GenericMethod genericMethod, ValueArray<DefiniteParameter> parameters, DefiniteBoundGenericType definiteBoundGenericType)
    {
        return new DefiniteMethod(parameters, genericMethod.Name, GetDefiniteType(genericMethod.ContainedType, definiteBoundGenericType.TypeArguments), definiteBoundGenericType.TypeArguments, genericMethod.IsConstructor);
    }

    private static DefiniteType GetDefiniteType(ContaineeType containeeType, ValueArray<DefiniteTypeArgument> typeArguments)
    {
        return containeeType switch
        {
            ContaineeType.GenericType genericType => genericType.ToDefiniteBoundGenericType(typeArguments),
            ContaineeType.NamedType namedType => DefiniteType.NamedType(namedType.Name, namedType.Namespace, namedType.AssemblyName),
        };
    }
}