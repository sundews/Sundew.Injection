// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScopeResolverBuilder.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Resolvers;

using System;
using System.Collections.Generic;
using Sundew.Base.Equality;
using Sundew.DiscriminatedUnions;
using Sundew.Injection.Generator.Stages.FactoryDataStage.TypeSystem;
using Sundew.Injection.Generator.TypeSystem;
using Type = Sundew.Injection.Generator.TypeSystem.Type;

internal sealed class ScopeResolverBuilder
{
    private readonly BindingResolver bindingResolver;
    private readonly Dictionary<Binding, Scope> bindingScopes;
    private readonly Dictionary<Type, Scope> externalParameterScopes;

    public ScopeResolverBuilder(BindingResolver bindingResolver)
    : this(bindingResolver, new Dictionary<Binding, Scope>(ReferenceEqualityComparer<Binding>.Instance), new Dictionary<Type, Scope>())
    {
    }

    internal ScopeResolverBuilder(BindingResolver bindingResolver, Dictionary<Binding, Scope> bindingScopes, Dictionary<Type, Scope> externalParameterScopes)
    {
        this.bindingResolver = bindingResolver;
        this.bindingScopes = bindingScopes;
        this.externalParameterScopes = externalParameterScopes;
    }

    public Scope UpdateScope(Binding binding, Scope parentScope)
    {
        var scope = PickScope(binding.Scope, parentScope);
        if (this.bindingScopes.TryGetValue(binding, out var previousResolvedScope))
        {
            scope = PickScope(scope, previousResolvedScope);
            this.bindingScopes[binding] = scope;
        }
        else
        {
            this.bindingScopes.Add(binding, scope);
        }

        return scope;
    }

    public Scope UpdateScope(Type type, Scope parentScope)
    {
        var scope = PickScope(Scope.Auto, parentScope);
        if (this.externalParameterScopes.TryGetValue(type, out var previousResolvedScope))
        {
            scope = PickScope(scope, previousResolvedScope);
            this.externalParameterScopes[type] = scope;
        }
        else
        {
            this.externalParameterScopes.Add(type, scope);
        }

        return scope;
    }

    public ScopeResolver Build(Binding binding)
    {
        this.ResolveBindingScopes(ResolvedBinding.SingleParameter(binding), Scope.NewInstance);
        return new ScopeResolver(this.bindingScopes, this.externalParameterScopes);
    }

    private static Scope PickScope(Scope suggestedScope, Scope parentScope)
    {
        return suggestedScope switch
        {
            Scope.AutoScope => parentScope,
            Scope.NewInstanceScope => parentScope,
            Scope.SingleInstancePerRequestScope => parentScope == Scope.NewInstance ? suggestedScope : parentScope,
            Scope.SingleInstancePerFuncResultScope => parentScope == Scope.NewInstance ||
                                             parentScope == Scope.SingleInstancePerRequest
                ? suggestedScope
                : parentScope,
            Scope.SingleInstancePerFactoryScope => parentScope == Scope.NewInstance ||
                                                       parentScope == Scope.SingleInstancePerRequest ||
                                                       parentScope is Scope.SingleInstancePerFuncResultScope
                ? suggestedScope
                : parentScope,
        };
    }

    private void ResolveBindingScopes(ResolvedBinding resolvedBinding, Scope parentScope)
    {
        void PickBindingScope(Binding binding, Scope parentScope)
        {
            var scope = this.UpdateScope(binding, parentScope);

            foreach (var parameter in binding.Method.Parameters)
            {
                this.ResolveBindingScopes(this.bindingResolver.ResolveBinding(parameter.Type, parameter.TypeMetadata), scope);
            }
        }

        switch (resolvedBinding)
        {
            case SingleParameter singleParameter:
                PickBindingScope(singleParameter.Binding, parentScope);
                break;
            case ArrayParameter arrayParameter:
                foreach (var binding in arrayParameter.Bindings)
                {
                    PickBindingScope(binding, parentScope);
                }

                this.UpdateScope(arrayParameter.ArrayType, parentScope);

                break;
            case ExternalParameter externalParameter:
                this.UpdateScope(externalParameter.Type, parentScope);
                break;
            case Error error:
                break;
        }
    }
}