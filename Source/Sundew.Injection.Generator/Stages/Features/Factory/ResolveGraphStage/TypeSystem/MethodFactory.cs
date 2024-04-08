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
        static Item<Parameter, Symbol> LookupOpenGenericType(OpenGenericType openGenericType, string parameterName, TypeMetadata typeMetadata, Dictionary<TypeParameter, TypeArgument> typeParameterDictionary)
        {
            var lookupResult = openGenericType.TypeParameters.AllOrFailed(x => LookupTypeParameter(x, x.Name, typeParameterDictionary));
            if (lookupResult.IsSuccess)
            {
                return Item.Pass(new Parameter(openGenericType.ToClosedGenericType(lookupResult.Value.Items.Select(x => new TypeArgument(x.Type, x.TypeMetadata)).ToValueArray()), parameterName, typeMetadata, ParameterNecessity._Required));
            }

            return Item.Fail<Parameter, Symbol>(openGenericType);
        }

        static Item<Parameter, Symbol> LookupTypeParameter(TypeParameter typeParameter, string parameterName, Dictionary<TypeParameter, TypeArgument> typeParameterDictionary)
        {
            return typeParameterDictionary.TryGetValue(typeParameter, out var typeArgument)
                ? Item.Pass(new Parameter(typeArgument.Type, parameterName, typeArgument.TypeMetadata, ParameterNecessity._Required))
                : Item.Fail<Parameter, Symbol>(typeParameter);
        }

        static Item<Parameter, Symbol> LookupTypeParameterArray(TypeParameterArray typeParameterArray, string parameterName, Dictionary<TypeParameter, TypeArgument> typeParameterDictionary)
        {
            if (typeParameterDictionary.TryGetValue(typeParameterArray.ElementTypeParameter, out var typeArgument))
            {
                return Item.Pass(
                    new Parameter(
                        new ArrayType(typeArgument.Type),
                        parameterName,
                        new TypeMetadata(null, new EnumerableMetadata(true, true, true), false),
                        ParameterNecessity._Required));
            }

            return Item.Fail<Parameter, Symbol>(typeParameterArray);
        }

        var typeParameterDictionary = openGenericType.TypeParameters
            .Zip(closedGenericType.TypeArguments, (symbol, typeArgument) => (symbol, typeArgument))
            .ToDictionary(tuple => tuple.symbol, tuple => tuple.typeArgument);
        var methodParameters = genericMethod.Parameters.AllOrFailed(
            x =>
            {
                return x.Type switch
                {
                    NamedType namedType => Item.Pass(new Parameter(namedType, x.Name, x.TypeMetadata, ParameterNecessity._Required)),
                    OpenGenericType openGenericType => LookupOpenGenericType(openGenericType, x.Name, x.TypeMetadata, typeParameterDictionary),
                    TypeParameter typeParameter => LookupTypeParameter(typeParameter, x.Name, typeParameterDictionary),
                    TypeParameterArray typeParameterArray => LookupTypeParameterArray(typeParameterArray, x.Name, typeParameterDictionary),
                    ClosedGenericType closedGenericType => Item.Pass(new Parameter(closedGenericType, x.Name, x.TypeMetadata, ParameterNecessity._Required)),
                    NestedType nestedType => Item.Pass(new Parameter(nestedType, x.Name, x.TypeMetadata, ParameterNecessity._Required)),
                    ArrayType arrayType => Item.Pass(new Parameter(arrayType, x.Name, x.TypeMetadata, ParameterNecessity._Required)),
                };
            });

        return methodParameters.With(
            all => CreateMethod(genericMethod, all.Items.ToImmutableArray(), closedGenericType),
            failed => new CreateGenericMethodError(genericMethod.Name, genericMethod.ContainingType, failed.Items.Select(x => (x.Item, x.Error)).ToImmutableArray()));
    }

    private static Method CreateMethod(GenericMethod genericMethod, ValueArray<Parameter> parameters, ClosedGenericType closedGenericType)
    {
        return new Method(GetType(genericMethod.ContainingType, closedGenericType.TypeArguments), genericMethod.Name, parameters, closedGenericType.TypeArguments, genericMethod.Kind);
    }

    private static Type GetType(ContaineeType containeeType, ValueArray<TypeArgument> typeArguments)
    {
        return containeeType switch
        {
            ContaineeType.GenericType genericType => genericType.ToClosedGenericType(typeArguments),
            ContaineeType.NamedType namedType => Type.NamedType(namedType.Name, namedType.Namespace, namedType.AssemblyName, namedType.IsValueType),
        };
    }
}