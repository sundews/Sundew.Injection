// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImplementFactoryVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Base;
using Sundew.Base.Collections;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Collections.Linq;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;
using Accessibility = Sundew.Injection.Accessibility;
using ISymbol = Microsoft.CodeAnalysis.ISymbol;
using Type = Sundew.Injection.Generator.TypeSystem.Type;

internal class ImplementFactoryVisitor(
    GenericNameSyntax genericNameSyntax,
    IMethodSymbol methodSymbol,
    AnalysisContext analysisContext,
    Location location)
    : CSharpSyntaxWalker
{
    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        var parameters = methodSymbol.Parameters;
        var i = 1;
        var factoryMethods = new FactoryMethodRegistrationBuilder();
        var accessibilityParameter = parameters[i++];
        var accessibility = accessibilityParameter.HasExplicitDefaultValue ? accessibilityParameter.ExplicitDefaultValue?.ToEnumOrDefault(Accessibility.Public) ?? Accessibility.Public : Accessibility.Public;
        var argumentIndex = 0;
        foreach (var argumentSyntax in node.Arguments)
        {
            if (argumentSyntax.NameColon != null)
            {
                switch (argumentSyntax.NameColon.Name.ToString())
                {
                    case nameof(factoryMethods):
                        new FactoryMethodVisitor(factoryMethods, analysisContext).Visit(argumentSyntax);
                        break;
                    case nameof(accessibility):
                        accessibility = analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value.ToEnumOrDefault(Accessibility.Public);
                        break;
                }
            }
            else
            {
                switch (argumentIndex)
                {
                    case 0:
                        new FactoryMethodVisitor(factoryMethods, analysisContext).Visit(argumentSyntax);
                        break;
                    case 1:
                        accessibility = analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value.ToEnumOrDefault(Accessibility.Public);
                        break;
                }

                argumentIndex++;
            }
        }

        var typeArguments = methodSymbol.MapTypeArguments(genericNameSyntax);
        var factoryTypeSymbol = typeArguments[0];
        var factoryType = analysisContext.TypeFactory.GetNamedType(factoryTypeSymbol);
        TypeSymbolWithLocation? factoryInterfaceTypeSymbol = typeArguments.Length == 2 ? typeArguments[1] : null;
        R<NamedType, TypeSymbolWithLocation>? factoryInterfaceTypeResult = factoryInterfaceTypeSymbol.HasValue ? analysisContext.TypeFactory.GetNamedType(factoryInterfaceTypeSymbol.Value) : null;
        DeclaredConstructor constructor = new DeclaredConstructor(ValueArray<FullParameter>.Empty, false, false);
        var factoryParameterSourcesBuilder = ImmutableDictionary.CreateBuilder<TypeId, (List<ParameterSource> ParameterSources, ScopeContext ScopeContext)>();
        if (factoryTypeSymbol.TypeSymbol.GetMembers().Where(x => x.IsStatic).OfType<IMethodSymbol>().TryGetOnlyOne(x => x!.Name == "Constructor", out var factoryConstructor))
        {
            if (analysisContext.TypeFactory.GetFactoryMethod(factoryConstructor, true).TryGetError(out var error, out var factoryConstructorMethod))
            {
                analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(
                    new ErrorWithLocation(
                    error,
                    factoryConstructor.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax(CancellationToken.None).GetLocation() ?? Location.None));
                return;
            }

            constructor = new DeclaredConstructor(
                factoryConstructorMethod.Parameters,
                factoryConstructor.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Public,
                factoryConstructor.IsPartialDefinition);

            var scope = new ScopeContext(Scope._SingleInstancePerFactory(Location.None), ScopeSelection.Default);
            this.AddParametersAndProperties(factoryConstructor.Parameters, scope, true, factoryParameterSourcesBuilder);
        }

        if (factoryType.TryGetError(out var invalidFactoryTypeSymbol))
        {
            analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostics.InvalidFactoryTypeError, invalidFactoryTypeSymbol);
            return;
        }

        this.AddFactoryMethods(factoryMethods, factoryTypeSymbol, factoryInterfaceTypeSymbol);

        NamedType? factoryInterfaceType = default;
        if (factoryInterfaceTypeResult.TryGetValue(out var result)
            && !result.TryGet(out factoryInterfaceType, out var invalidFactoryInterfaceTypeSymbol))
        {
            analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostics.InvalidFactoryTypeError, invalidFactoryInterfaceTypeSymbol);
            return;
        }

        analysisContext.CompiletimeInjectionDefinitionBuilder.ImplementFactory(
            factoryType.Value,
            factoryInterfaceType,
            constructor,
            GetParameterSourceContexts(factoryParameterSourcesBuilder),
            factoryMethods,
            accessibility,
            location);
    }

    private static bool IsOptional(NullableAnnotation nullableAnnotation, ITypeSymbol typeSymbol)
    {
        return nullableAnnotation == NullableAnnotation.Annotated || typeSymbol.SpecialType == SpecialType.System_Nullable_T;
    }

    private static ImmutableDictionary<TypeId, ParameterSourceContexts> GetParameterSourceContexts(ImmutableDictionary<TypeId, (List<ParameterSource> ParameterSources, ScopeContext ScopeContext)>.Builder factoryParameterSourcesBuilder)
    {
        return factoryParameterSourcesBuilder.ToImmutableDictionary(x => x.Key, x => new ParameterSourceContexts(x.Value.ParameterSources.ToArray(), x.Value.ScopeContext));
    }

    private void AddFactoryMethods(FactoryMethodRegistrationBuilder factoryMethodRegistrationBuilder, TypeSymbolWithLocation factoryTypeSymbol, TypeSymbolWithLocation? factoryInterfaceTypeSymbol)
    {
        static bool IsEligibleMember(ISymbol x)
        {
            return x is { IsStatic: false, Kind: SymbolKind.Method } || x.Kind == SymbolKind.Property;
        }

        var allTypes = factoryTypeSymbol.TypeSymbol.AllInterfaces;
        if (factoryInterfaceTypeSymbol.TryGetValue(out var interfaceTypeSymbol) && interfaceTypeSymbol.TypeSymbol is INamedTypeSymbol interfaceNamedTypeSymbol)
        {
            allTypes = allTypes.Add(interfaceNamedTypeSymbol);
        }

        if (factoryTypeSymbol.TypeSymbol is INamedTypeSymbol factoryNamedTypeSymbol)
        {
            allTypes = allTypes.Add(factoryNamedTypeSymbol);
        }

        var factoryDeclarations = allTypes.Select(typeSymbol => (typeSymbol, namedType: analysisContext.TypeFactory.GetNamedType(typeSymbol))).DistinctByInOrder(x => x.namedType)
            .Select(x => (members: x.typeSymbol.GetMembers().Where(IsEligibleMember).ToArray(), x.namedType))
            .Where(x => x.members.Length > 0).ToArray();

        foreach (var (members, factoryType) in factoryDeclarations)
        {
            var factoryDeclarationAndMethods = ImmutableArray.CreateBuilder<FactoryMethodRegistration>();
            var factoryMethodTargets = ImmutableArray.CreateBuilder<FactoryMethodTarget>();

            foreach (var member in members)
            {
                var memberResult = member switch
                {
                    IMethodSymbol methodSymbol => R.Success(
                        (methodSymbol.ReturnType,
                        Scope: Scope._NewInstance(factoryTypeSymbol.Location),
                        FactoryMethodTarget: AnalysisContextExtensions.GetFactoryMethodTarget(
                            analysisContext,
                            methodSymbol),
                        methodSymbol.Parameters)).Omits<Error>(),
                    IPropertySymbol propertySymbol => R.Success(
                        (propertySymbol.Type,
                        Scope._SingleInstancePerFactory(factoryTypeSymbol.Location),
                        AnalysisContextExtensions.GetFactoryMethodTarget(analysisContext, propertySymbol),
                        ImmutableArray<IParameterSymbol>.Empty)),
                    _ => R.Error(new Error(ErrorType.UnsupportedSymbol, new NamedSymbol(member.ToDisplayString()), [])),
                };

                if (memberResult.TryGetError(out var error, out var tuple))
                {
                    analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(new ErrorWithLocation(error, factoryTypeSymbol.Location));
                    continue;
                }

                var (returnTypeSymbol, scope, factoryMethodTargetResultOption, parameterSymbols) = tuple;
                if (!factoryMethodTargetResultOption.TryGetValue(out var factoryTargetMethodResult))
                {
                    continue;
                }

                if (factoryTargetMethodResult.TryGetError(out var factoryTargetMethodError, out var factoryMethodTarget))
                {
                    analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(new ErrorWithLocation(factoryTargetMethodError, factoryTypeSymbol.Location));
                    continue;
                }

                if (analysisContext.TypeFactory.GetFullType(returnTypeSymbol).TryGetError(out var returnTypeError, out var returnType))
                {
                    analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(new ErrorWithLocation(returnTypeError, factoryTypeSymbol.Location));
                }
                else
                {
                    factoryMethodTargets.Add(factoryMethodTarget);
                    var factoryParameterSourcesBuilder = ImmutableDictionary.CreateBuilder<TypeId, (List<ParameterSource> ParameterSources, ScopeContext ScopeContext)>();
                    var scope2 = new ScopeContext(Scope._SingleInstancePerRequest(Location.None), ScopeSelection.Default);
                    this.AddParametersAndProperties(parameterSymbols, scope2, false, factoryParameterSourcesBuilder);

                    var accessibility = member.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Public
                        ? Accessibility.Public
                        : Accessibility.Internal;
                    var bindingRegistrations =
                        analysisContext.CompiletimeInjectionDefinitionBuilder.TryGetBindingRegistrations(returnType.Type);
                    switch (bindingRegistrations.ByCardinality())
                    {
                        case Empty<BindingRegistration> empty:
                            if (returnTypeSymbol.IsInstantiable() && returnType.DefaultConstructor.TryGetValue(out var method))
                            {
                                analysisContext.CompiletimeInjectionDefinitionBuilder.Bind(ImmutableArray.Create(returnType.Type), returnType, method, new ScopeContext(Scope._Auto, ScopeSelection.Implicit), false, false);
                                factoryDeclarationAndMethods.Add(
                                    new FactoryMethodRegistration(
                                        returnType,
                                        returnType,
                                        new ScopeContext(scope, ScopeSelection.Implicit),
                                        method,
                                        accessibility,
                                        false,
                                        default,
                                        factoryMethodTarget,
                                        GetParameterSourceContexts(factoryParameterSourcesBuilder)));
                            }
                            else
                            {
                                analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(
                                    Diagnostics.NoBindingFoundForNonConstructableTypeError, factoryTypeSymbol, returnType.Type.FullName);
                            }

                            break;
                        case Single<BindingRegistration> single:
                            factoryDeclarationAndMethods.Add(
                                new FactoryMethodRegistration(
                                    returnType,
                                    single.Item.TargetType,
                                    single.Item.Scope,
                                    single.Item.Method,
                                    accessibility,
                                    false,
                                    default,
                                    factoryMethodTarget,
                                    GetParameterSourceContexts(factoryParameterSourcesBuilder)));
                            break;
                        case Multiple<BindingRegistration> multiple:
                            if (multiple.Items.TryGetOnlyOne(
                                    x => x != null && factoryMethodTarget.Method.Name.Contains(x.TargetType.Type.Name),
                                    out var bindingRegistration))
                            {
                                factoryDeclarationAndMethods.Add(
                                    new FactoryMethodRegistration(
                                    returnType,
                                    bindingRegistration.TargetType,
                                    bindingRegistration.Scope,
                                    bindingRegistration.Method,
                                    accessibility,
                                    false,
                                    default,
                                    factoryMethodTarget,
                                    GetParameterSourceContexts(factoryParameterSourcesBuilder)));
                            }

                            break;
                    }
                }
            }

            factoryMethodRegistrationBuilder.Add(factoryType, factoryDeclarationAndMethods.ToImmutable());
        }
    }

    private void AddParametersAndProperties(
        ImmutableArray<IParameterSymbol> parameterSymbols,
        ScopeContext scope,
        bool isForConstructor,
        ImmutableDictionary<TypeId, (List<ParameterSource> ParameterSources, ScopeContext ScopeContext)>.Builder parameterSourcesBuilder)
    {
        static bool IsFunc(INamedTypeSymbol namedTypeSymbol, AnalysisContext analysisContext)
        {
            return namedTypeSymbol.IsGenericType && SymbolEqualityComparer.Default.Equals(namedTypeSymbol.OriginalDefinition, analysisContext.KnownAnalysisTypes.FuncTypeSymbol);
        }

        static bool IsEligibleForMembers(ITypeSymbol type)
        {
            return type.SpecialType switch
            {
                SpecialType.System_Collections_Generic_IList_T => false,
                SpecialType.System_Collections_Generic_IReadOnlyList_T => false,
                SpecialType.System_Collections_Generic_IReadOnlyCollection_T => false,
                SpecialType.System_Collections_Generic_ICollection_T => false,
                SpecialType.System_Collections_Generic_IEnumerator_T => false,
                SpecialType.System_Collections_Generic_IEnumerable_T => false,
                SpecialType.System_Collections_IEnumerable => false,
                SpecialType.System_Collections_IEnumerator => false,
                SpecialType.System_String => false,
                _ => true,
            };
        }

        var isAccessedAsMember = scope.Scope is Scope.SingleInstancePerFactory;
        foreach (var parameterSymbol in parameterSymbols)
        {
            var parameterType = analysisContext.TypeFactory.GetType(parameterSymbol.Type);
            if (IsEligibleForMembers(parameterSymbol.Type))
            {
                foreach (var accessorProperty in parameterSymbol.Type.GetMembers().OfType<IPropertySymbol>()
                             .Where(x => !x.IsStatic && x.GetMethod != null && x.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Public).Select(x =>
                             {
                                 var propertyType = analysisContext.TypeFactory.GetType(x.Type);
                                 var resultType = propertyType;
                                 var isFunc = false;
                                 if (x.Type is INamedTypeSymbol namedTypeSymbol && IsFunc(namedTypeSymbol, analysisContext))
                                 {
                                     isFunc = true;
                                     resultType = analysisContext.TypeFactory.GetType(namedTypeSymbol.TypeArguments.First());
                                 }

                                 return (isFunc, Accessor: new AccessorProperty(analysisContext.TypeFactory.GetNamedType(x.ContainingType), IsOptional(parameterSymbol.NullableAnnotation, parameterSymbol.Type), resultType, propertyType, IsOptional(x.NullableAnnotation, x.Type), x.Name));
                             }))
                {
                    if (accessorProperty.isFunc)
                    {
                        if (scope.Selection == ScopeSelection.Default)
                        {
                            scope = new ScopeContext(Scope._NewInstance(Location.None), ScopeSelection.Implicit);
                        }

                        this.AddParameterSource(accessorProperty.Accessor.ResultType, ParameterSource.PropertyAccessorParameter(parameterType, parameterSymbol.MetadataName, isAccessedAsMember, accessorProperty.Accessor, true), scope, parameterSourcesBuilder);
                    }

                    this.AddParameterSource(accessorProperty.Accessor.PropertyType, ParameterSource.PropertyAccessorParameter(parameterType, parameterSymbol.MetadataName, isAccessedAsMember, accessorProperty.Accessor, false), scope, parameterSourcesBuilder);
                }
            }

            this.AddParameterSource(parameterType, ParameterSource.DirectParameter(parameterType, parameterSymbol.MetadataName, isAccessedAsMember, false, TypeConverter.GetParameterNecessity(parameterSymbol, false), Inject.Shared), scope, parameterSourcesBuilder);
            if (parameterSymbol.Type is INamedTypeSymbol parameterNamedTypeSymbol && IsFunc(parameterNamedTypeSymbol, analysisContext))
            {
                var resultType = analysisContext.TypeFactory.GetType(parameterNamedTypeSymbol.TypeArguments.First());
                this.AddParameterSource(resultType, ParameterSource.DirectParameter(parameterType, parameterSymbol.MetadataName, isAccessedAsMember, true, TypeConverter.GetParameterNecessity(parameterSymbol, false), Inject.Shared), scope, parameterSourcesBuilder);
            }

            if (SymbolEqualityComparer.Default.Equals(parameterSymbol.Type.WithNullableAnnotation(NullableAnnotation.None), analysisContext.KnownAnalysisTypes.ILifecycleParameters))
            {
                this.AddParameterSource(analysisContext.TypeFactory.GetType(analysisContext.KnownAnalysisTypes.InitializationParameters), ParameterSource.DirectParameter(parameterType, parameterSymbol.MetadataName, isAccessedAsMember, false, TypeConverter.GetParameterNecessity(parameterSymbol, false), Inject.Shared), scope, parameterSourcesBuilder);
                this.AddParameterSource(analysisContext.TypeFactory.GetType(analysisContext.KnownAnalysisTypes.DisposalParameters), ParameterSource.DirectParameter(parameterType, parameterSymbol.MetadataName, isAccessedAsMember, false, TypeConverter.GetParameterNecessity(parameterSymbol, false), Inject.Shared), scope, parameterSourcesBuilder);
            }
        }
    }

    private void AddParameterSource(Type parameterType, ParameterSource parameterSource, ScopeContext scopeContext, ImmutableDictionary<TypeId, (List<ParameterSource> ParameterSources, ScopeContext ScopeContext)>.Builder parameterSourcesBuilder)
    {
        var parameterTypeId = parameterType.Id;
        if (!parameterSourcesBuilder.TryGetValue(parameterTypeId, out var parameterSourcesTuple))
        {
            parameterSourcesTuple.ParameterSources = [];
            parameterSourcesTuple.ScopeContext = scopeContext;
            parameterSourcesBuilder.Add(parameterTypeId, parameterSourcesTuple);
        }

        parameterSourcesTuple.ParameterSources.Add(parameterSource);
    }
}
