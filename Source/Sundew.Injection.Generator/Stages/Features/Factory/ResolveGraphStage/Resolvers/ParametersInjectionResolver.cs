// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequiredParametersInjectionResolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;

using System.Linq;
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

internal class ParametersInjectionResolver
{
    private readonly ParametersInjectionResolver? externalParametersInjectionResolver;

    public ParametersInjectionResolver(
        ValueDictionary<TypeId, ParameterSourceContexts> parameterSourceContexts,
        ParametersInjectionResolver? externalParametersInjectionResolver = null)
    {
        this.externalParametersInjectionResolver = externalParametersInjectionResolver;
        this.ParameterSourceContexts = parameterSourceContexts;
    }

    public ValueDictionary<TypeId, ParameterSourceContexts> ParameterSourceContexts { get; }

    public ResolvedParameterSource ResolveParameterSource(Type type, string name, Type? dependantTypeOption)
    {
        ValueArray<ParameterSource> parameterSources = ValueArray<ParameterSource>.Empty;
        if (this.externalParametersInjectionResolver is not null && this.externalParametersInjectionResolver.ParameterSourceContexts.TryGetValue(type.Id, out var externalParameterSources))
        {
            parameterSources = externalParameterSources.ParameterSources;
        }

        if (this.ParameterSourceContexts.TryGetValue(type.Id, out var ownParameterSources))
        {
            parameterSources = parameterSources.AddRange(ownParameterSources.ParameterSources);
        }

        return this.ResolveParameterSource(type, name, dependantTypeOption, parameterSources);
    }

    private ResolvedParameterSource ResolveParameterSource(Type type, string name, Type? dependantTypeOption, ValueArray<ParameterSource> parameterSources)
    {
        switch (parameterSources.Count)
        {
            case 0:
                return ResolvedParameterSource._NotFound(new DirectParameter(type, name, false, false, ParameterNecessity._Required, Inject.Shared));
            case 1:
                return ResolvedParameterSource._Found(parameterSources[0]);
            default:
                name = name.Uncapitalize();
                var parameterSource = parameterSources.FirstOrDefault(
                    x =>
                    {
                        return x switch
                        {
                            DirectParameter directParameter => directParameter.Name == name || (dependantTypeOption.HasValue && directParameter.Name.Contains(dependantTypeOption.Name)),
                            PropertyAccessorParameter propertyAccessorParameter => propertyAccessorParameter
                                .AccessorProperty.Name.Uncapitalize() == name || (dependantTypeOption.HasValue && propertyAccessorParameter.AccessorProperty.Name.Contains(dependantTypeOption.Name)),
                        };
                    });
                if (parameterSource != null)
                {
                    return ResolvedParameterSource._Found(parameterSource);
                }

                return ResolvedParameterSource._NoExactMatch(type, name, parameterSources);
        }
    }
}