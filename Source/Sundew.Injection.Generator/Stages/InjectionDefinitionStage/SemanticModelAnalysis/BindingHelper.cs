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
using Sundew.Injection.Generator.TypeSystem;
using MethodKind = Sundew.Injection.Generator.TypeSystem.MethodKind;

internal static class BindingHelper
{
    public static void BindFactory(this AnalysisContext analysisContext, (Type Type, TypeMetadata TypeMetadata) factoryType, IEnumerable<(Method Method, ITypeSymbol ReturnType)> createMethods)
    {
        if (!analysisContext.CompiletimeInjectionDefinitionBuilder.HasBinding(factoryType.Type) &&
            factoryType.TypeMetadata.DefaultConstructor.TryGetValue(out var defaultConstructor))
        {
            var actualMethod = new Method(defaultConstructor.Parameters, factoryType.Type.Name, factoryType.Type, MethodKind._Constructor);
            analysisContext.CompiletimeInjectionDefinitionBuilder.Bind(ImmutableArray<(Type Type, TypeMetadata TypeMetadata)>.Empty, factoryType, actualMethod, Scope._SingleInstancePerFactory, false, false);
        }

        foreach (var methodAndReturnType in createMethods)
        {
            var returnType = analysisContext.TypeFactory.CreateType(methodAndReturnType.ReturnType);
            analysisContext.CompiletimeInjectionDefinitionBuilder.Bind(ImmutableArray<(Type Type, TypeMetadata TypeMetadata)>.Empty, returnType, methodAndReturnType.Method, Scope._Auto, false, false);

            if (SymbolEqualityComparer.Default.Equals(methodAndReturnType.ReturnType.OriginalDefinition, analysisContext.KnownAnalysisTypes.ConstructedTypeSymbol))
            {
                analysisContext.CompiletimeInjectionDefinitionBuilder.Bind(
                    ImmutableArray<(Type Type, TypeMetadata TypeMetadata)>.Empty,
                    analysisContext.TypeFactory.CreateType(((INamedTypeSymbol)methodAndReturnType.ReturnType)
                        .TypeArguments.Single()),
                    new Method(
                        nameof(Constructed<object>.Object),
                        analysisContext.TypeFactory.CreateType(methodAndReturnType.ReturnType).Type,
                        MethodKind._Instance(returnType.TypeMetadata with { HasLifetime = false }, true)),
                    Scope._Auto,
                    false,
                    false);
            }
        }
    }
}