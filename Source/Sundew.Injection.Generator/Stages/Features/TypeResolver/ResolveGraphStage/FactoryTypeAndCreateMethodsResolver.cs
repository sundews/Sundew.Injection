// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryTypeAndCreateMethodsResolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.TypeResolver.ResolveGraphStage;

using System.Linq;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;
using FactoryMethod = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.FactoryMethod;

internal class FactoryTypeAndCreateMethodsResolver(
    ICache<string, ValueArray<FactoryMethod>> typeRegistry)
{
    public (Type Type, ValueArray<FactoryMethod> CreateMethods) ResolveFactoryRegistration(FactoryRegistration factoryRegistration)
    {
        var createMethods = this.ResolveCreateMethods(factoryRegistration.FactoryType);
        if (createMethods.IsEmpty)
        {
            var createMethodResults = factoryRegistration.FactoryMethods.Select(factoryMethod =>
            {
                var returnType = factoryMethod.ReturnType;
                var parameters = factoryMethod.Parameters;

                return new FactoryMethod(
                    factoryMethod.Name,
                    parameters.Select(parameter => new ParameterDeclaration(parameter.Type, parameter.Name)).ToValueList(),
                    returnType);
            });

            return (factoryRegistration.FactoryType, createMethodResults.ToValueArray());
        }

        return (factoryRegistration.FactoryType, createMethods);
    }

    private ValueArray<FactoryMethod> ResolveCreateMethods(Type type)
    {
        return typeRegistry.TryGet(type.Name, out var createMethods) ? createMethods : ValueArray<FactoryMethod>.Empty;
    }
}