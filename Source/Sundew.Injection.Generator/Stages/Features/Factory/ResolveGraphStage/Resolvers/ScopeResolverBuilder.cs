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
    private readonly Dictionary<RequestedParameter, (bool IsOptional, bool Unused)> references = new();

    public ScopeResolverBuilder(
        BindingResolver bindingResolver,
        ValueDictionary<TypeId, ParameterSourceContexts> parameterSources,
        ValueArray<FactoryImplementationDefinition> factoryImplementationDefinitions)
        : this(
            bindingResolver,
            parameterSources.ToDictionary(
                x => x.Key,
                x => new ScopeContext(x.Value.ScopeContext.Scope, x.Value.ScopeContext.Selection)),
            factoryImplementationDefinitions)
    {
    }

    internal ScopeResolverBuilder(
        BindingResolver bindingResolver,
        Dictionary<TypeId, ScopeContext> scopes,
        ValueArray<FactoryImplementationDefinition> factoryImplementationDefinitions)
    {
        this.bindingResolver = bindingResolver;
        this.scopes = scopes;
        foreach (var factoryImplementationDefinition in factoryImplementationDefinitions)
        {
            var scopeContext = new ScopeContext(Scope._SingleInstancePerRequest(Location.None), ScopeSelection.Implicit);
            this.scopes.Add(factoryImplementationDefinition.FactoryType.Id, scopeContext);
            if (factoryImplementationDefinition.FactoryInterfaceType.TryGetValue(out var factoryInterfaceType))
            {
                this.scopes.Add(factoryInterfaceType.Id, scopeContext);
            }
        }
    }

    public R<ScopeResolver, ImmutableList<ResolvedBindingError>> Build(Type factoryType, Binding binding, ParametersInjectionResolver parametersInjectionResolver)
    {
        var errors = ImmutableList.CreateBuilder<ResolvedBindingError>();
        this.ResolveBindingScopes(ResolvedBinding.SingleParameter(binding), default, parametersInjectionResolver, new Dependant(factoryType, Scope._NewInstance(Location.None)), errors);
        return R.From(errors.IsEmpty(), new ScopeResolver(this.scopes), errors.ToImmutable());
    }

    public ValueDictionary<RequestedParameter, bool> Build()
    {
        return this.references.ToImmutableDictionary(x => x.Key, x => x.Value.IsOptional);
    }

    private ScopeContext UpdateBindingScope(Binding binding, Dependant dependant, ImmutableList<ResolvedBindingError>.Builder errors)
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

        errors.AddIfHasValue(scopeResult.Error);
        return scopeResult.Context;
    }

    private ScopeContext UpdateParameterScope(Type type, Dependant dependant, ImmutableList<ResolvedBindingError>.Builder errors)
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

        errors.AddIfHasValue(scopeResult.Error);
        return scopeResult.Context;
    }

    private void UpdateReferencedType(Type type, RequestedParameterMetadata? requestedParameterMetadataOption, bool parameterCanBeProvided, ImmutableList<ResolvedBindingError>.Builder errors)
    {
        if (requestedParameterMetadataOption is not { } requestedParameterMetadata)
        {
            return;
        }

        var typeId = type.Id;
        var requestedParameter = new RequestedParameter(typeId, requestedParameterMetadata.Name);
        if (this.references.TryGetValue(requestedParameter, out var previousTargetReference))
        {
            var isOptionalResult = (previousTargetReference.IsOptional, requestedParameterMetadata.ParameterNecessity.IsOptional) switch
            {
                (true, true) => R.Success(true),
                (false, true) => R.From(parameterCanBeProvided, () => false, () => ResolvedBindingError.ParameterError(type, requestedParameterMetadata.Name, ValueArray<ParameterSource>.Empty)),
                (true, false) => R.Success(false),
                (false, false) => R.Success(false),
            };

            if (isOptionalResult.IsError)
            {
                errors.Add(isOptionalResult.Error);
                return;
            }

            previousTargetReference.IsOptional = isOptionalResult.Value;
        }
        else
        {
            this.references.Add(requestedParameter, (requestedParameterMetadata.ParameterNecessity.IsOptional, false));
        }
    }

    private void ResolveBindingScopes(ResolvedBinding resolvedBinding, RequestedParameterMetadata? requestedParameterMetadataOption, ParametersInjectionResolver parametersInjectionResolver, Dependant dependant, ImmutableList<ResolvedBindingError>.Builder errors)
    {
        void PickBindingScope(Binding binding)
        {
            var scopeContext = this.UpdateBindingScope(binding, dependant, errors);

            var nextDependant = new Dependant(binding.TargetType, scopeContext.Scope);
            if (binding.Method.Kind is MethodKind.Instance instance)
            {
                this.ResolveBindingScopes(
                    this.bindingResolver.ResolveBinding(binding.Method.ContainingType, instance.ContainingTypeMetadata, instance.ContainingTypeDefaultConstructor, default, default, parametersInjectionResolver),
                    default,
                    parametersInjectionResolver,
                    nextDependant,
                    errors);
            }

            foreach (var parameter in binding.Method.Parameters)
            {
                this.ResolveBindingScopes(
                    this.bindingResolver.ResolveBinding(parameter.Type, parameter.TypeMetadata, parameter.DefaultConstructor, new(parameter.Name, parameter.ParameterNecessity), nextDependant.Type, parametersInjectionResolver),
                    new(parameter.Name, parameter.ParameterNecessity),
                    parametersInjectionResolver,
                    nextDependant,
                    errors);
            }
        }

        switch (resolvedBinding)
        {
            case ThisFactoryParameter thisFactoryParameter:
                this.UpdateReferencedType(thisFactoryParameter.FactoryType, requestedParameterMetadataOption, true, errors);
                this.UpdateParameterScope(thisFactoryParameter.FactoryType, dependant with { Scope = Scope._SingleInstancePerFactory(Location.None) }, errors);
                if (thisFactoryParameter.FactoryInterfaceType.HasValue())
                {
                    this.UpdateReferencedType(thisFactoryParameter.FactoryInterfaceType, requestedParameterMetadataOption, true, errors);
                    this.UpdateParameterScope(thisFactoryParameter.FactoryInterfaceType, dependant with { Scope = Scope._SingleInstancePerFactory(Location.None) }, errors);
                }

                break;
            case SingleParameter singleParameter:
                PickBindingScope(singleParameter.Binding);
                this.UpdateReferencedType(singleParameter.Binding.ReferencedType, requestedParameterMetadataOption, true, errors);
                break;
            case MultiItemParameter multiItemParameter:
                foreach (var binding in multiItemParameter.Bindings)
                {
                    PickBindingScope(binding);
                }

                this.UpdateParameterScope(multiItemParameter.Type, dependant, errors);
                this.UpdateReferencedType(multiItemParameter.Type, requestedParameterMetadataOption, true, errors);
                break;
            case OptionalParameter defaultParameter:
                this.UpdateReferencedType(defaultParameter.Type, requestedParameterMetadataOption, false, errors);
                this.UpdateParameterScope(defaultParameter.Type, dependant with { Scope = Scope._NewInstance(Location.None) }, errors);
                break;
            case RequiredParameter requiredParameter:
                this.UpdateReferencedType(requiredParameter.Type, requestedParameterMetadataOption, false, errors);
                this.UpdateParameterScope(requiredParameter.Type, dependant, errors);
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