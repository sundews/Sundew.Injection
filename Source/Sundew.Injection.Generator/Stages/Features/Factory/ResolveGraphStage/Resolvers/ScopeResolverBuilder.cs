// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScopeResolverBuilder.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;

using System.Collections.Generic;
using System.Collections.Immutable;
using Sundew.Base;
using Sundew.Base.Collections;
using Sundew.Base.Equality;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;
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
        var scope = PickScope(Scope._Auto, parentScope);
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

    public R<ScopeResolver, ImmutableList<ResolvedBindingError>> Build(Binding binding)
    {
        var errors = ImmutableList.CreateBuilder<ResolvedBindingError>();
        this.ResolveBindingScopes(ResolvedBinding.SingleParameter(binding), Scope._NewInstance, errors);
        return R.From(errors.IsEmpty(), new ScopeResolver(this.bindingScopes, this.externalParameterScopes), errors.ToImmutable());
    }

    private static Scope PickScope(Scope suggestedScope, Scope dependeeScope)
    {
        return suggestedScope switch
        {
            Scope.Auto => dependeeScope,
            Scope.NewInstance => dependeeScope,
            Scope.SingleInstancePerRequest => dependeeScope == Scope._NewInstance ? suggestedScope : dependeeScope,
            Scope.SingleInstancePerFuncResult => dependeeScope == Scope._NewInstance ||
                                             dependeeScope == Scope._SingleInstancePerRequest
                ? suggestedScope
                : dependeeScope,
            Scope.SingleInstancePerFactory => dependeeScope == Scope._NewInstance ||
                                                       dependeeScope == Scope._SingleInstancePerRequest ||
                                                       dependeeScope is Scope.SingleInstancePerFuncResult
                ? suggestedScope
                : dependeeScope,
        };
    }

    private void ResolveBindingScopes(ResolvedBinding resolvedBinding, Scope dependeeScope, ImmutableList<ResolvedBindingError>.Builder errors)
    {
        void PickBindingScope(Binding binding, Scope dependeeScope)
        {
            var scope = this.UpdateScope(binding, dependeeScope);
            if (binding.Method.Kind is MethodKind.Instance instance)
            {
                this.ResolveBindingScopes(this.bindingResolver.ResolveBinding(binding.Method.ContainingType, instance.ContainingTypeMetadata, null), scope, errors);
            }

            foreach (var parameter in binding.Method.Parameters)
            {
                this.ResolveBindingScopes(this.bindingResolver.ResolveBinding(parameter.Type, parameter.TypeMetadata, (parameter.Name, parameter.ParameterNecessity)), scope, errors);
            }
        }

        switch (resolvedBinding)
        {
            case SingleParameter singleParameter:
                if (singleParameter.Binding.Method.Kind is MethodKind.Instance instance)
                {
                    this.ResolveBindingScopes(this.bindingResolver.ResolveBinding(singleParameter.Binding.Method.ContainingType, instance.ContainingTypeMetadata, null), dependeeScope, errors);

                    this.UpdateScope(singleParameter.Binding.Method.ContainingType, dependeeScope);
                }

                PickBindingScope(singleParameter.Binding, dependeeScope);
                break;
            case MultiItemParameter multiItemParameter:
                foreach (var binding in multiItemParameter.Bindings)
                {
                    PickBindingScope(binding, dependeeScope);
                }

                this.UpdateScope(multiItemParameter.Type, dependeeScope);

                break;
            case DefaultParameter defaultParameter:
                this.UpdateScope(defaultParameter.Type, Scope._NewInstance);
                break;
            case ExternalParameter externalParameter:
                this.UpdateScope(externalParameter.Type, dependeeScope);
                break;
            case ResolvedBindingError.ParameterError parameterError:
                errors.Add(parameterError);
                break;
            case ResolvedBindingError.Error error:
                errors.Add(error);
                break;
        }
    }
}