// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;
using MethodKind = Sundew.Injection.Generator.TypeSystem.MethodKind;

internal static class BindingHelper
{
    public static void BindFactory(
        this AnalysisContext analysisContext,
        FullType factoryType,
        IEnumerable<(Method Method, TypeSymbolWithLocation ReturnType)> factoryMethods)
    {
        if (!analysisContext.CompiletimeInjectionDefinitionBuilder.HasBinding(factoryType.Type) &&
            factoryType.DefaultConstructor.TryGetValue(out var defaultConstructor))
        {
            var actualMethod = new Method(factoryType.Type, factoryType.Type.Name, defaultConstructor.Parameters, ValueArray<FullTypeArgument>.Empty, MethodKind._Constructor);
            analysisContext.CompiletimeInjectionDefinitionBuilder.Bind(ImmutableArray<Type>.Empty, factoryType, actualMethod, new ScopeContext(Scope._SingleInstancePerFactory(Location.None), ScopeSelection.Implicit), false, false);
        }

        foreach (var methodAndReturnType in factoryMethods)
        {
            var returnTypeResult = analysisContext.TypeFactory.GetFullType(methodAndReturnType.ReturnType);
            if (returnTypeResult.IsError)
            {
                analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostics.InfiniteRecursionError, returnTypeResult.Error);
                return;
            }

            var returnType = returnTypeResult.Value;
            analysisContext.CompiletimeInjectionDefinitionBuilder.Bind(ImmutableArray<Type>.Empty, returnType, methodAndReturnType.Method, new ScopeContext(Scope._Auto, ScopeSelection.Implicit), false, false);

            if (SymbolEqualityComparer.Default.Equals(methodAndReturnType.ReturnType.TypeSymbol.OriginalDefinition, analysisContext.KnownAnalysisTypes.ConstructedTypeSymbol))
            {
                var typeSymbol = ((INamedTypeSymbol)methodAndReturnType.ReturnType.TypeSymbol).TypeArguments.Single();
                var returnTypeFirstTypeParameterResult = analysisContext.TypeFactory.GetFullType(typeSymbol);
                if (returnTypeFirstTypeParameterResult.IsError)
                {
                    analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostics.InfiniteRecursionError, methodAndReturnType.ReturnType with { TypeSymbol = typeSymbol }, returnTypeFirstTypeParameterResult.Error.GetErrorText());
                    return;
                }

                analysisContext.CompiletimeInjectionDefinitionBuilder.Bind(
                    ImmutableArray<Type>.Empty,
                    returnTypeFirstTypeParameterResult.Value,
                    new Method(
                        returnType.Type,
                        nameof(Constructed<object>.Object),
                        ValueArray<FullParameter>.Empty,
                        ValueArray<FullTypeArgument>.Empty,
                        MethodKind._Instance(returnType.Metadata with { HasLifetime = false }, true, returnType.DefaultConstructor)),
                    new ScopeContext(Scope._Auto, ScopeSelection.Implicit),
                    false,
                    false);
            }
        }
    }
}