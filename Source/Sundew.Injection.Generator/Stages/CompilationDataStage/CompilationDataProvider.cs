// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompilationDataProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CompilationDataStage;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.CodeAnalysis;
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Collections.Linq;
using Sundew.Injection.Dependencies;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;
using MethodKind = Sundew.Injection.Generator.TypeSystem.MethodKind;
using Type = System.Type;

internal static class CompilationDataProvider
{
    private const string ObjectName = "object";
    private const string IntName = "int";
    private static readonly NamedType VoidType = new("void", string.Empty, string.Empty, true);
    private static readonly NamedType ValueTaskType = new("ValueTask", "System.Threading.Tasks", string.Empty, true);

    public static IncrementalValueProvider<R<CompilationData, Diagnostics>> SetupCompilationDataStage(this IncrementalValueProvider<Compilation> compilationProvider)
    {
        return compilationProvider.Select(GetCompilationData);
    }

    internal static R<CompilationData, Diagnostics> GetCompilationData(Compilation compilation, CancellationToken cancellationToken)
    {
        var areNullableAnnotationsSupported = compilation.Options.NullableContextOptions != NullableContextOptions.Disable;

        var requiredTypeSymbols = new[]
        {
            compilation.GetIInitializableTypeSymbol(),
            compilation.GetIAsyncInitializableTypeSymbol(),
            compilation.GetIDisposableTypeSymbol(),
            compilation.GetIAsyncDisposableTypeSymbol(),
            compilation.GetFunc(),
            compilation.GetTask(),
            compilation.GetIEnumerableOfT(),
            compilation.GetIReadOnlyListOfT(),
        }.AllOrFailed();

        cancellationToken.ThrowIfCancellationRequested();
        if (requiredTypeSymbols.TryGet(out var all, out var errors))
        {
            var assemblyNamespace = TypeHelper.GetAssemblySundewInjectionDependenciesNamespace(compilation);
            var assemblyName = compilation.AssemblyName ?? string.Empty;

            var index = 0;
            var iInitializableType = TypeConverter.GetNamedType(all[index++]);
            var iAsyncInitializableType = TypeConverter.GetNamedType(all[index++]);

            var iDisposableType = TypeConverter.GetNamedType(all[index++]);
            var iAsyncDisposableType = TypeConverter.GetNamedType(all[index++]);

            var func = all[index++];
            var task = all[index++];

            var iEnumerableOfTSymbol = all[index++];
            var iReadOnlyListOfTSymbol = all[index++];
            var lifecycleHandlerType = GetProvidedNamedType(typeof(LifecycleHandler), assemblyNamespace, assemblyName);
            var lifecycleHandlerConstructorParameters = typeof(LifecycleHandler).GetConstructors().Where(x => x.IsPublic && !x.IsStatic).Select(x => x.GetParameters()).FirstOrDefault(x => x.Length == 2);
            var defaultMetadata = new TypeMetadata(EnumerableMetadata.NonEnumerableMetadata, false);
            var lifecycleHandlerBinding = new Binding(
                lifecycleHandlerType,
                lifecycleHandlerType,
                new ScopeContext(Scope._SingleInstancePerFactory(Location.None), ScopeSelection.Implicit),
                new Method(
                    lifecycleHandlerType,
                    lifecycleHandlerType.Name,
                    lifecycleHandlerConstructorParameters!.Select(x => new FullParameter(GetNamedType(x.ParameterType), x.Name, defaultMetadata, default, ParameterNecessity._Optional(null))).ToValueArray(),
                    ImmutableArray<FullTypeArgument>.Empty,
                    MethodKind._Constructor),
                false,
                false,
                false);
            var objectType = new NamedType(ObjectName, string.Empty, string.Empty, false);
            var intType = new NamedType(IntName, string.Empty, string.Empty, false);
            var resolverItemType = GetProvidedNamedType(typeof(ResolverItem), assemblyNamespace, assemblyName);
            return R.Success(new CompilationData(
                areNullableAnnotationsSupported,
                iInitializableType,
                iAsyncInitializableType,
                iDisposableType,
                iAsyncDisposableType,
                VoidType,
                ValueTaskType,
                GenericTypeConverter.GetGenericType(task),
                GenericTypeConverter.GetGenericType(func),
                GetNamedType(typeof(Type)),
                objectType,
                intType,
                GetNamedType(typeof(IServiceProvider)),
                GetGenericType(typeof(Span<>), string.Empty).ToClosedGenericType(ImmutableArray.Create(new FullTypeArgument(objectType, new TypeMetadata(EnumerableMetadata.NonEnumerableMetadata, false)))),
                GenericTypeConverter.GetGenericType(iEnumerableOfTSymbol),
                GenericTypeConverter.GetGenericType(iEnumerableOfTSymbol),
                new SundewInjectionReferencedCompilationData(
                    GetNamedType(typeof(ILifecycleHandler)),
                    GetGenericType(typeof(Sundew.Injection.Constructed<>), string.Empty),
                    GetNamedType(typeof(IGeneratedFactory))),
                new SundewInjectionProvidedCompilationData(
                    lifecycleHandlerBinding,
                    GetProvidedNamedType(typeof(ResolverItemsFactory), assemblyNamespace, assemblyName),
                    resolverItemType,
                    new ArrayType(resolverItemType),
                    GetGenericType(typeof(ResolverItem), assemblyNamespace, assemblyName)),
                assemblyName,
                assemblyNamespace));
        }

        return R.Error(new Diagnostics(errors.Select(x => Diagnostic.Create(Diagnostics.RequiredTypeNotFoundError, null, x.Error))));
    }

    private static NamedType GetProvidedNamedType(Type type, string @namespace, string assemblyName)
    {
        return new NamedType(type.Name, @namespace, assemblyName, type.IsValueType);
    }

    private static NamedType GetNamedType(Type type)
    {
        return GetProvidedNamedType(type, type.Namespace, type.Assembly.GetName().FullName);
    }

    private static OpenGenericType GetGenericType(System.Type genericType, string assemblyName)
    {
        return GetGenericType(genericType, genericType.Namespace, assemblyName);
    }

    private static OpenGenericType GetGenericType(System.Type genericType, string @namespace, string assemblyName)
    {
        var name = GetGenericName(genericType);
        return new OpenGenericType(
            name,
            @namespace,
            assemblyName,
            genericType.GetTypeInfo().GenericTypeParameters.Select(x => new TypeParameter(x.Name)).ToImmutableArray(),
            genericType.IsValueType);
    }

    private static string GetGenericName(Type genericType)
    {
        var name = genericType.Name;
        var genericIndex = genericType.Name.IndexOf('`');
        if (genericIndex > -1)
        {
            name = name.Substring(0, genericIndex);
        }

        return name;
    }
}