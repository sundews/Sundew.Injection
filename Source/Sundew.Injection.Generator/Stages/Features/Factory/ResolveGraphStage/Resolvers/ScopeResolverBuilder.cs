// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScopeResolverBuilder.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Sundew.Base;
using Sundew.Base.Collections;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;
using Type = Sundew.Injection.Generator.TypeSystem.Type;

internal sealed class ScopeResolverBuilder
{
    private readonly BindingResolver bindingResolver;
    private readonly Dictionary<TypeId, ScopeContext> scopes;

    public ScopeResolverBuilder(
        BindingResolver bindingResolver,
        ValueDictionary<TypeId, (Scope Scope, ScopeOrigin Origin)> requiredParameterScopes,
        ValueArray<FactoryCreationDefinition> factoryCreationDefinitions)
    : this(
        bindingResolver,
        requiredParameterScopes.ToDictionary(
            x => x.Key,
            x => new ScopeContext { Scope = x.Value.Scope, Origin = x.Value.Origin }),
        factoryCreationDefinitions)
    {
    }

    internal ScopeResolverBuilder(
        BindingResolver bindingResolver,
        Dictionary<TypeId, ScopeContext> scopes,
        ValueArray<FactoryCreationDefinition> factoryCreationDefinitions)
    {
        this.bindingResolver = bindingResolver;
        this.scopes = scopes;
        foreach (var factoryCreationDefinition in factoryCreationDefinitions)
        {
            var scope = new ScopeContext { Scope = Scope._SingleInstancePerRequest, Origin = ScopeOrigin.Explicit };
            this.scopes.Add(factoryCreationDefinition.FactoryType.Id, scope);
            if (factoryCreationDefinition.FactoryInterfaceType.TryGetValue(out var factoryInterfaceType))
            {
                this.scopes.Add(factoryInterfaceType.Id, scope);
            }
        }
    }

    public Scope UpdateBindingScope(Binding binding, Scope parentScope)
    {
        var typeId = binding.ReferencedType.Id;
        var scope = ScopePicker.Pick(binding.Scope, parentScope);
        if (this.scopes.TryGetValue(typeId, out var previousResolvedScope))
        {
            scope = ScopePicker.Pick(scope, previousResolvedScope.Scope);
            previousResolvedScope.Scope = scope;
            previousResolvedScope.Origin = ScopeOrigin.Implicit;
        }
        else
        {
            var scopePair = new ScopeContext { Scope = scope, Origin = ScopeOrigin.Implicit };
            this.scopes.Add(typeId, scopePair);
            this.scopes[binding.TargetType.Id] = scopePair;
        }

        return scope;
    }

    public Scope UpdateParameterScope(Type type, Scope parentScope)
    {
        var typeId = type.Id;
        var scope = Scope._NewInstance;
        if (this.scopes.TryGetValue(typeId, out var previousResolveScope))
        {
            scope = ScopePicker.Pick(
                previousResolveScope.Origin == ScopeOrigin.Explicit
                    ? previousResolveScope.Scope
                    : scope,
                parentScope);
            previousResolveScope.Scope = scope;
        }
        else
        {
            scope = ScopePicker.Pick(scope, parentScope);
            this.scopes.Add(typeId, new ScopeContext { Scope = scope, Origin = ScopeOrigin.Implicit });
        }

        return scope;
    }

    public R<ScopeResolver, ImmutableList<ResolvedBindingError>> Build(Binding binding)
    {
        var errors = ImmutableList.CreateBuilder<ResolvedBindingError>();
        this.ResolveBindingScopes(ResolvedBinding.SingleParameter(binding), Scope._NewInstance, errors);
        return R.From(errors.IsEmpty(), new ScopeResolver(this.scopes), errors.ToImmutable());
    }

    private void ResolveBindingScopes(ResolvedBinding resolvedBinding, Scope dependeeScope, ImmutableList<ResolvedBindingError>.Builder errors)
    {
        void PickBindingScope(Binding binding, Scope dependeeScope)
        {
            var scope = this.UpdateBindingScope(binding, dependeeScope);
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
            case ThisFactoryParameter thisFactoryParameter:
                this.UpdateParameterScope(thisFactoryParameter.FactoryType, Scope._SingleInstancePerFactory);
                if (thisFactoryParameter.FactoryInterfaceType.HasValue())
                {
                    this.UpdateParameterScope(thisFactoryParameter.FactoryInterfaceType, Scope._SingleInstancePerFactory);
                }

                break;
            case SingleParameter singleParameter:
                if (singleParameter.Binding.Method.Kind is MethodKind.Instance instance)
                {
                    this.ResolveBindingScopes(this.bindingResolver.ResolveBinding(singleParameter.Binding.Method.ContainingType, instance.ContainingTypeMetadata, null), dependeeScope, errors);

                    this.UpdateBindingScope(singleParameter.Binding, dependeeScope);
                }

                PickBindingScope(singleParameter.Binding, dependeeScope);
                break;
            case MultiItemParameter multiItemParameter:
                foreach (var binding in multiItemParameter.Bindings)
                {
                    PickBindingScope(binding, dependeeScope);
                }

                this.UpdateParameterScope(multiItemParameter.Type, dependeeScope);

                break;
            case OptionalParameter defaultParameter:
                this.UpdateParameterScope(defaultParameter.Type, Scope._NewInstance);
                break;
            case RequiredParameter externalParameter:
                this.UpdateParameterScope(externalParameter.Type, dependeeScope);
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