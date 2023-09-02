// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeResolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.TypeSystem;

using System.Collections.Immutable;
using System.Linq;
using Sundew.Base.Collections;
using Sundew.Base.Primitives.Computation;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Resolvers;
using Sundew.Injection.Generator.TypeSystem;

internal sealed class TypeResolver
{
    private readonly ICache<string, NamedType> typeRegistry;

    public TypeResolver(ICache<string, NamedType> typeRegistry)
    {
        this.typeRegistry = typeRegistry;
    }

    public R<DefiniteType, FailedResolve> ResolveType(Type type)
    {
        return type switch
        {
            ArrayType arrayType => this.ResolveArrayType(arrayType),
            BoundGenericType boundGenericType => this.ResolveBoundGenericType(boundGenericType),
            NamedType namedType => R.Success<DefiniteType>(namedType),
            ErrorType errorType => this.TryResolveErrorType(errorType),
            DefiniteBoundGenericType definiteBoundGenericType => R.Success<DefiniteType>(definiteBoundGenericType),
            DefiniteArrayType definiteArrayType => R.Success<DefiniteType>(definiteArrayType),
        };
    }

    private R<DefiniteType, FailedResolve> ResolveBoundGenericType(BoundGenericType boundGenericType)
    {
        var result = boundGenericType.TypeArguments.AllOrFailed(argument =>
        {
            var result = this.ResolveType(argument.Type);
            return result.IsSuccess
                ? Item.Pass(new DefiniteTypeArgument(result.Value, argument.TypeMetadata))
                : Item.Fail<DefiniteTypeArgument, FailedResolve>(result.Error);
        });

        return result switch
        {
            All<TypeArgument, DefiniteTypeArgument, FailedResolve> all => R.Success<DefiniteType>(new DefiniteBoundGenericType(boundGenericType.Name, boundGenericType.Namespace, boundGenericType.AssemblyName, boundGenericType.TypeParameters, all.Items)),
            Failed<TypeArgument, DefiniteTypeArgument, FailedResolve> failed => R.Error(new FailedResolve(boundGenericType, failed.Items.Select(x => x.Error).ToImmutableArray())),
        };
    }

    private R<DefiniteType, FailedResolve> ResolveArrayType(ArrayType arrayType)
    {
        var result = this.ResolveType(arrayType.ElementType);
        if (result.IsSuccess)
        {
            return R.Success<DefiniteType>(new DefiniteArrayType(result.Value));
        }

        return R.Error(result.Error);
    }

    private R<DefiniteType, FailedResolve> TryResolveErrorType(ErrorType errorType)
    {
        return this.typeRegistry.TryGet(errorType.Name, out var resolvableType) ? R.Success<DefiniteType>(resolvableType) : R.Error(new FailedResolve(errorType));
    }
}