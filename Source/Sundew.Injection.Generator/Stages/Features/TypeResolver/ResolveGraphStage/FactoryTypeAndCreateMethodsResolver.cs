// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryTypeAndCreateMethodsResolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.TypeResolver.ResolveGraphStage;

using System.Collections.Immutable;
using System.Linq;
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Collections.Linq;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

internal class FactoryTypeAndCreateMethodsResolver(
    ICache<string, (DefiniteType DefiniteType, ValueArray<DefiniteFactoryMethod> Methods)> typeRegistry)
{
    private readonly ICache<string, (DefiniteType DefiniteType, ValueArray<DefiniteFactoryMethod> CreateMethods)> typeRegistry = typeRegistry;

    public R<(DefiniteType DefiniteType, ValueArray<DefiniteFactoryMethod> CreateMethods), FailedResolve> ResolveFactoryRegistration(FactoryRegistration factoryRegistration)
    {
        var factoryTypeResult = this.ResolveType(factoryRegistration.FactoryType);
        if (factoryTypeResult.TryGet(out var resolvedFactoryType, out var failedResolve))
        {
            if (resolvedFactoryType.CreateMethods.IsEmpty)
            {
                var createMethodResults = factoryRegistration.FactoryMethods.AllOrFailed(factoryMethod =>
                {
                    var returnTypeResult = this.ResolveType(factoryMethod.ReturnType).With(x => x.DefiniteType);
                    var parametersResult = factoryMethod.Parameters.AllOrFailed(x => this.ResolveType(x.Type).With(y => (x.Name, y.DefiniteType)).ToItem());

                    return (returnTypeResult, parametersResult) switch
                    {
                        ({ IsSuccess: true } returnTypeSuccess, { IsSuccess: true } parameterSuccess) =>
#if NULLABLE_ANALYZER_BUG
                            Item.Pass(new DefiniteFactoryMethod(factoryMethod.Name, parameterSuccess.Value.Select(parameter => new ParameterDeclaration(parameter.DefiniteType, parameter.Name)).ToValueList(), returnTypeSuccess.Value)),
#else
                            Item.Pass(new DefiniteFactoryMethod(factoryMethod.Name, parameterSuccess.Value.Select(parameter => new ParameterDeclaration(parameter.DefiniteType, parameter.Name)).ToValueList(), returnTypeSuccess.Value!)),
#endif
                        _ =>
                            Item.Fail<DefiniteFactoryMethod, FailedResolve>(new FailedResolve(factoryMethod.ReturnType, parametersResult.IfError(ImmutableList<FailedResolve>.Empty, (list, items) => list.AddRange(items.GetErrors())))),
                    };
                });

                return createMethodResults.With(
                    x =>
                    (resolvedFactoryType.DefiniteType, x.Items.ToValueArray()),
                    error => new FailedResolve(factoryRegistration.FactoryType, error.GetErrors()));
            }

            return R.Success(resolvedFactoryType);
        }

        return R.Error(new FailedResolve(factoryRegistration.FactoryType));
    }

    private R<(DefiniteType DefiniteType, ValueArray<DefiniteFactoryMethod> CreateMethods), FailedResolve> ResolveType(Type type)
    {
        return type switch
        {
            DefiniteType definiteType => R.Success((definiteType, ValueArray<DefiniteFactoryMethod>.Empty)),
            ArrayType arrayType => this.ResolveArrayType(arrayType),
            ClosedGenericType boundGenericType => this.ResolveClosedGenericType(boundGenericType),
            NestedType nestedType => this.ResolveNestedType(nestedType),
            ErrorType errorType => this.TryResolveErrorType(errorType),
        };
    }

    private R<(DefiniteType DefiniteType, ValueArray<DefiniteFactoryMethod> CreateMethods), FailedResolve> ResolveNestedType(NestedType nestedType)
    {
        var resolveContainingTypeResult = this.ResolveType(nestedType.ContainingType);
        var resolveContainedTypeResult = this.ResolveType(nestedType.ContainedType);
        var resolveResults = new[] { resolveContainedTypeResult, resolveContainingTypeResult }.AllOrFailed();
        if (resolveResults.TryGet(out var all, out var failed))
        {
            return R.Success((DefiniteType.DefiniteNestedType(all[0].DefiniteType, all[1].DefiniteType), ValueArray<DefiniteFactoryMethod>.Empty));
        }

        return R.Error(new FailedResolve(nestedType, failed.GetErrors()));
    }

    private R<(DefiniteType Type, ValueArray<DefiniteFactoryMethod> CreateMethods), FailedResolve> ResolveClosedGenericType(ClosedGenericType closedGenericType)
    {
        var result = closedGenericType.TypeArguments.AllOrFailed(argument =>
        {
            var result = this.ResolveType(argument.Type);
            return result.IsSuccess
                ? Item.Pass(new DefiniteTypeArgument(result.Value.DefiniteType, argument.TypeMetadata))
                : Item.Fail<DefiniteTypeArgument, FailedResolve>(result.Error);
        });

        return result.With(
            all => (DefiniteType.DefiniteClosedGenericType(closedGenericType.Name, closedGenericType.Namespace, closedGenericType.AssemblyName, closedGenericType.TypeParameters, all.Items, closedGenericType.IsValueType), ValueArray<DefiniteFactoryMethod>.Empty),
            failed => new FailedResolve(closedGenericType, failed.Items.Select(x => x.Error).ToImmutableArray()));
    }

    private R<(DefiniteType Type, ValueArray<DefiniteFactoryMethod> CreateMethods), FailedResolve> ResolveArrayType(ArrayType arrayType)
    {
        return this.ResolveType(arrayType.ElementType).With(x => (DefiniteType.DefiniteArrayType(x.DefiniteType), x.CreateMethods));
    }

    private R<(DefiniteType Type, ValueArray<DefiniteFactoryMethod> CreateMethods), FailedResolve> TryResolveErrorType(ErrorType errorType)
    {
        return this.typeRegistry.TryGet(errorType.Name, out var resolvableType) ? R.Success(resolvableType) : R.Error(new FailedResolve(errorType));
    }
}