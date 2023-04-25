// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequiredParametersInjectionResolver.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Resolvers;

using System.Linq;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Primitives.Computation;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

internal class RequiredParametersInjectionResolver
{
    public RequiredParametersInjectionResolver(Inject inject, ValueDictionary<Type, ValueArray<ParameterSource>> injectTypes)
    {
        this.Inject = inject;
        this.InjectTypes = injectTypes;
    }

    public Inject Inject { get; }

    public ValueDictionary<Type, ValueArray<ParameterSource>> InjectTypes { get; }

    public ResolvedParameterSource ResolveParameterSource(Type type, string name)
    {
        if (this.InjectTypes.TryGetValue(type, out var parameterSources))
        {
            return this.ResolveParameterSource(type, name, parameterSources);
        }

        return new ResolvedParameterSource.Found(ParameterSource.DirectParameter(this.Inject));
    }

    private ResolvedParameterSource ResolveParameterSource(Type type, string name, ValueArray<ParameterSource> parameterSources)
    {
        name = name.Uncapitalize();
        switch (parameterSources.Count)
        {
            case 0:
                return new ResolvedParameterSource.Found(ParameterSource.DirectParameter(this.Inject));
            case 1:
                return new ResolvedParameterSource.Found(parameterSources[0]);
            default:
                var parameterSource = parameterSources.FirstOrDefault(
                    x =>
                    {
                        if (x is PropertyAccessorParameter property)
                        {
                            return property.AccessorProperty.Name.Uncapitalize() == name;
                        }

                        return false;
                    });
                if (parameterSource != null)
                {
                    return new ResolvedParameterSource.Found(parameterSource);
                }

                return new ResolvedParameterSource.NoExactMatch(type, name, parameterSources);
        }
    }
}