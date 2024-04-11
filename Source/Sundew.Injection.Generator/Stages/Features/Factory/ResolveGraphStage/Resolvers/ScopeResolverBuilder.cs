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
using Microsoft.CodeAnalysis;
using Sundew.Base;
using Sundew.Base.Collections;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;
using MethodKind = Sundew.Injection.Generator.TypeSystem.MethodKind;

internal sealed class ScopeResolverBuilder
{
    private readonly BindingResolver bindingResolver;

    private readonly Dictionary<TypeId, ScopeContext> scopes;

    public ScopeResolverBuilder(
        BindingResolver bindingResolver,
        ValueDictionary<TypeId, InjectionDefinitionStage.ScopeContext> requiredParameterScopes,
        ValueArray<FactoryCreationDefinition> factoryCreationDefinitions)
        : this(
            bindingResolver,
            requiredParameterScopes.ToDictionary(
                x => x.Key,
                x => new ScopeContext(x.Value.Scope, x.Value.Selection)),
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
            var scopeContext = new ScopeContext(Scope._SingleInstancePerRequest(Location.None), ScopeSelection.Implicit);
            this.scopes.Add(factoryCreationDefinition.FactoryType.Id, scopeContext);
            if (factoryCreationDefinition.FactoryInterfaceType.TryGetValue(out var factoryInterfaceType))
            {
                this.scopes.Add(factoryInterfaceType.Id, scopeContext);
            }
        }
    }

    public ScopeContext UpdateBindingScope(Binding binding, Dependant dependant, ImmutableList<ResolvedBindingError>.Builder errors)
    {
        var typeId = binding.ReferencedType.Id;
        var scopeResult = (Context: new ScopeContext(binding.Scope.Scope, binding.Scope.Selection), Error: default(ScopeError));
        if (this.scopes.TryGetValue(typeId, out var previousResolvedScope))
        {
            scopeResult = ScopePicker.Pick(binding.TargetType, previousResolvedScope, dependant);
            previousResolvedScope.Scope = scopeResult.Context.Scope;
        }
        else
        {
            scopeResult = ScopePicker.Pick(binding.TargetType, scopeResult.Context, dependant);
            this.scopes.Add(typeId, scopeResult.Context);
            this.scopes[binding.TargetType.Id] = scopeResult.Context;
        }

        errors.TryAdd(scopeResult.Error);
        return scopeResult.Context;
    }

    public ScopeContext UpdateParameterScope(Type type, Dependant dependant, ImmutableList<ResolvedBindingError>.Builder errors)
    {
        var typeId = type.Id;
        var scopeResult = (Context: new ScopeContext(Scope._NewInstance(Location.None), ScopeSelection.Implicit), Error: default(ScopeError));
        if (this.scopes.TryGetValue(typeId, out var previousResolveScope))
        {
            scopeResult = ScopePicker.Pick(type, previousResolveScope, dependant);
            previousResolveScope.Scope = scopeResult.Context.Scope;
        }
        else
        {
            scopeResult = ScopePicker.Pick(type, scopeResult.Context, dependant);
            this.scopes.Add(typeId, scopeResult.Context);
        }

        errors.TryAdd(scopeResult.Error);
        return scopeResult.Context;
    }

    public R<ScopeResolver, ImmutableList<ResolvedBindingError>> Build(Type factoryType, Binding binding)
    {
        var errors = ImmutableList.CreateBuilder<ResolvedBindingError>();
        this.ResolveBindingScopes(ResolvedBinding.SingleParameter(binding), new Dependant(factoryType, Scope._NewInstance(Location.None)), errors);
        return R.From(errors.IsEmpty(), new ScopeResolver(this.scopes), errors.ToImmutable());
    }

    private void ResolveBindingScopes(ResolvedBinding resolvedBinding, Dependant dependant, ImmutableList<ResolvedBindingError>.Builder errors)
    {
        void PickBindingScope(Binding binding)
        {
            var scopeContext = this.UpdateBindingScope(binding, dependant, errors);

            var nextDependant = new Dependant(binding.TargetType, scopeContext.Scope);
            if (binding.Method.Kind is MethodKind.Instance instance)
            {
                this.ResolveBindingScopes(this.bindingResolver.ResolveBinding(binding.Method.ContainingType, instance.ContainingTypeMetadata, default), nextDependant, errors);
            }

            foreach (var parameter in binding.Method.Parameters)
            {
                this.ResolveBindingScopes(this.bindingResolver.ResolveBinding(parameter.Type, parameter.TypeMetadata, (parameter.Name, parameter.ParameterNecessity)), nextDependant, errors);
            }
        }

        switch (resolvedBinding)
        {
            case ThisFactoryParameter thisFactoryParameter:
                this.UpdateParameterScope(thisFactoryParameter.FactoryType, dependant with { Scope = Scope._SingleInstancePerFactory(default, Location.None) }, errors);
                if (thisFactoryParameter.FactoryInterfaceType.HasValue())
                {
                    this.UpdateParameterScope(thisFactoryParameter.FactoryInterfaceType, dependant with { Scope = Scope._SingleInstancePerFactory(default, Location.None) }, errors);
                }

                break;
            case SingleParameter singleParameter:
                if (singleParameter.Binding.Method.Kind is MethodKind.Instance instance)
                {
                    this.ResolveBindingScopes(this.bindingResolver.ResolveBinding(singleParameter.Binding.Method.ContainingType, instance.ContainingTypeMetadata, default), dependant, errors);

                    this.UpdateBindingScope(singleParameter.Binding, dependant, errors);
                }

                PickBindingScope(singleParameter.Binding);
                break;
            case MultiItemParameter multiItemParameter:
                foreach (var binding in multiItemParameter.Bindings)
                {
                    PickBindingScope(binding);
                }

                this.UpdateParameterScope(multiItemParameter.Type, dependant, errors);
                break;
            case OptionalParameter defaultParameter:
                this.UpdateParameterScope(defaultParameter.Type, dependant with { Scope = Scope._NewInstance(Location.None) }, errors);
                break;
            case RequiredParameter externalParameter:
                this.UpdateParameterScope(externalParameter.Type, dependant, errors);
                break;
            case ResolvedBindingError resolvedBindingError:
                errors.Add(resolvedBindingError);
                break;
        }
    }

    public sealed record ScopeContext(Scope Scope, ScopeSelection Selection)
    {
        public Scope Scope { get; set; } = Scope;

        public ScopeSelection Selection { get; set; } = Selection;
    }
}