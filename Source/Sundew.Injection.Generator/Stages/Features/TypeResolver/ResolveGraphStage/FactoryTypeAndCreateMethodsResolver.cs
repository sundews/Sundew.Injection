// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryTypeAndCreateMethodsResolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.TypeResolver.ResolveGraphStage;

using System.Collections.Immutable;
using System.Linq;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

internal class FactoryTypeAndCreateMethodsResolver(
    ICache<string, ValueArray<FactoryTargetDeclaration>> typeRegistry)
{
    public (Type Type, ValueArray<FactoryTargetDeclaration> FactoryTargets) ResolveFactoryRegistration(FactoryRegistration factoryRegistration)
    {
        var factoryTargets = this.ResolveFactoryTargets(factoryRegistration.FactoryType);
        if (factoryTargets.IsEmpty)
        {
            var factoryTargetResults = factoryRegistration.FactoryTargets.Select(factoryTarget =>
                {
                    return new FactoryTargetDeclaration(
                        factoryTarget.Method.Name,
                        factoryTarget.Method.Parameters.Select(parameter => new ParameterDeclaration(parameter.Type, parameter.Name, parameter.ParameterNecessity)).ToValueList(),
                        factoryTarget.ReturnType,
                        factoryTarget.Method.Kind is MethodKind.Instance { IsProperty: true },
                        ImmutableArray<string>.Empty);
                }).ToValueArray();

            return (factoryRegistration.FactoryType, factoryTargetResults);
        }

        return (factoryRegistration.FactoryType, factoryTargets);
    }

    private ValueArray<FactoryTargetDeclaration> ResolveFactoryTargets(Type type)
    {
        return typeRegistry.TryGet(type.Name, out var factoryTargets) ? factoryTargets : ValueArray<FactoryTargetDeclaration>.Empty;
    }
}