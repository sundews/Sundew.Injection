// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodFactory.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Collections.Linq;
using Sundew.Injection.Generator.TypeSystem;

internal sealed class MethodFactory
{
    public R<Method, CreateGenericMethodError> CreateMethod(ClosedGenericType closedGenericType, OpenGenericType openGenericType, GenericMethod genericMethod)
    {
        static Item<FullParameter, Symbol> LookupOpenGenericType(
            OpenGenericType openGenericType,
            string parameterName,
            TypeMetadata typeMetadata,
            Method? defaultConstructor,
            Dictionary<TypeParameter, FullTypeArgument> typeParameterDictionary)
        {
            var lookupResult = openGenericType.TypeParameters.AllOrFailed(x => LookupTypeParameter(x, x.Name, typeParameterDictionary));
            if (lookupResult.IsSuccess)
            {
                return Item.Pass(
                    new FullParameter(
                    openGenericType.ToClosedGenericType(lookupResult.Value.Items
                        .Select(x => new FullTypeArgument(x.Type, x.TypeMetadata)).ToValueArray()),
                    parameterName,
                    typeMetadata,
                    defaultConstructor,
                    ParameterNecessity._Required));
            }

            return Item.Fail<FullParameter, Symbol>(openGenericType);
        }

        static Item<FullParameter, Symbol> LookupTypeParameter(TypeParameter typeParameter, string parameterName, Dictionary<TypeParameter, FullTypeArgument> typeParameterDictionary)
        {
            return typeParameterDictionary.TryGetValue(typeParameter, out var typeArgument)
                ? Item.Pass(new FullParameter(typeArgument.Type, parameterName, typeArgument.TypeMetadata, default, ParameterNecessity._Required))
                : Item.Fail<FullParameter, Symbol>(typeParameter);
        }

        static Item<FullParameter, Symbol> LookupTypeParameterArray(TypeParameterArray typeParameterArray, string parameterName, Dictionary<TypeParameter, FullTypeArgument> typeParameterDictionary)
        {
            if (typeParameterDictionary.TryGetValue(typeParameterArray.ElementTypeParameter, out var typeArgument))
            {
                return Item.Pass(
                    new FullParameter(
                        new ArrayType(typeArgument.Type),
                        parameterName,
                        new TypeMetadata(new EnumerableMetadata(true, true, true), false),
                        default,
                        ParameterNecessity._Required));
            }

            return Item.Fail<FullParameter, Symbol>(typeParameterArray);
        }

        var typeParameterDictionary = openGenericType.TypeParameters
            .Zip(closedGenericType.TypeArguments, (symbol, typeArgument) => (symbol, typeArgument))
            .ToDictionary(tuple => tuple.symbol, tuple => tuple.typeArgument);
        var methodParameters = genericMethod.Parameters.AllOrFailed(
            x =>
            {
                return x.Type switch
                {
                    NamedType namedType => Item.Pass(new FullParameter(namedType, x.Name, x.TypeMetadata, x.DefaultConstructor, ParameterNecessity._Required)),
                    OpenGenericType openGenericType => LookupOpenGenericType(openGenericType, x.Name, x.TypeMetadata, x.DefaultConstructor, typeParameterDictionary),
                    TypeParameter typeParameter => LookupTypeParameter(typeParameter, x.Name, typeParameterDictionary),
                    TypeParameterArray typeParameterArray => LookupTypeParameterArray(typeParameterArray, x.Name, typeParameterDictionary),
                    ClosedGenericType closedGenericType => Item.Pass(new FullParameter(closedGenericType, x.Name, x.TypeMetadata, x.DefaultConstructor, ParameterNecessity._Required)),
                    NestedType nestedType => Item.Pass(new FullParameter(nestedType, x.Name, x.TypeMetadata, x.DefaultConstructor, ParameterNecessity._Required)),
                    ArrayType arrayType => Item.Pass(new FullParameter(arrayType, x.Name, x.TypeMetadata, x.DefaultConstructor, ParameterNecessity._Required)),
                };
            });

        return methodParameters.With(
            all => CreateMethod(genericMethod, all.Items.ToImmutableArray(), closedGenericType),
            failed => new CreateGenericMethodError(genericMethod.Name, genericMethod.ContainingType, failed.Items.Select(x => (x.Item, x.Error)).ToImmutableArray()));
    }

    private static Method CreateMethod(GenericMethod genericMethod, ValueArray<FullParameter> parameters, ClosedGenericType closedGenericType)
    {
        return new Method(GetType(genericMethod.ContainingType, closedGenericType.TypeArguments), genericMethod.Name, parameters, closedGenericType.TypeArguments, genericMethod.Kind);
    }

    private static Type GetType(ContaineeType containeeType, ValueArray<FullTypeArgument> typeArguments)
    {
        return containeeType switch
        {
            ContaineeType.GenericType genericType => genericType.ToClosedGenericType(typeArguments),
            ContaineeType.NamedType namedType => Type.NamedType(namedType.Name, namedType.Namespace, namedType.AssemblyName, namedType.IsValueType),
        };
    }
}