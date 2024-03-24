// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeResolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;

using System.Collections.Immutable;
using System.Linq;
using Sundew.Base;
using Sundew.Base.Collections.Linq;
using Sundew.Injection.Generator.Stages.Features;
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
            ClosedGenericType boundGenericType => this.ResolveClosedGenericType(boundGenericType),
            NamedType namedType => R.Success<DefiniteType>(namedType),
            NestedType nestedType => this.ResolveNestedType(nestedType),
            ErrorType errorType => this.TryResolveErrorType(errorType),
            DefiniteClosedGenericType definiteBoundGenericType => R.Success<DefiniteType>(definiteBoundGenericType),
            DefiniteNestedType definiteNestedType => R.Success<DefiniteType>(definiteNestedType),
            DefiniteArrayType definiteArrayType => R.Success<DefiniteType>(definiteArrayType),
        };
    }

    private R<DefiniteType, FailedResolve> ResolveNestedType(NestedType nestedType)
    {
        var resolveContainingTypeResult = this.ResolveType(nestedType.ContainingType);
        var resolveContainedTypeResult = this.ResolveType(nestedType.ContainedType);
        var resolveResults = new[] { resolveContainedTypeResult, resolveContainingTypeResult }.AllOrFailed();
        if (resolveResults.TryGet(out var all, out var failed))
        {
            return R.Success<DefiniteType>(new DefiniteNestedType(all[0], all[1]));
        }

        return R.Error(new FailedResolve(nestedType, failed.GetErrors()));
    }

    private R<DefiniteType, FailedResolve> ResolveClosedGenericType(ClosedGenericType closedGenericType)
    {
        var result = closedGenericType.TypeArguments.AllOrFailed(argument =>
        {
            var result = this.ResolveType(argument.Type);
            return result.IsSuccess
                ? Item.Pass(new DefiniteTypeArgument(result.Value, argument.TypeMetadata))
                : Item.Fail<DefiniteTypeArgument, FailedResolve>(result.Error);
        });

        return result.With(
            all => DefiniteType.DefiniteClosedGenericType(closedGenericType.Name, closedGenericType.Namespace, closedGenericType.AssemblyName, closedGenericType.TypeParameters, all.Items, closedGenericType.IsValueType),
            failed => new FailedResolve(closedGenericType, failed.Items.Select(x => x.Error).ToImmutableArray()));
    }

    private R<DefiniteType, FailedResolve> ResolveArrayType(ArrayType arrayType)
    {
        return this.ResolveType(arrayType.ElementType).With(DefiniteType.DefiniteArrayType);
    }

    private R<DefiniteType, FailedResolve> TryResolveErrorType(ErrorType errorType)
    {
        return this.typeRegistry.TryGet(errorType.Name, out var resolvableType) ? R.Success<DefiniteType>(resolvableType) : R.Error(new FailedResolve(errorType));
    }
}