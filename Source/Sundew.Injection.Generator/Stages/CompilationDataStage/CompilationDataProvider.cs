﻿// --------------------------------------------------------------------------------------------------------------------
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
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;
using Sundew.Injection.Generator.TypeSystem;
using MethodKind = Sundew.Injection.Generator.TypeSystem.MethodKind;
using Type = System.Type;

internal static class CompilationDataProvider
{
    private const string ObjectName = "object";
    private const string IntName = "int";
    private static readonly NamedType VoidType = new("void", string.Empty, string.Empty, true);
    private static readonly NamedType ValueTaskType = new("ValueTask", "System.Threading.Tasks", string.Empty, true);

    public static IncrementalValueProvider<R<CompilationData, ValueList<Diagnostic>>> SetupCompilationDataStage(this IncrementalValueProvider<Compilation> compilationProvider)
    {
        return compilationProvider.Select(GetCompilationData);
    }

    internal static R<CompilationData, ValueList<Diagnostic>> GetCompilationData(Compilation compilation, CancellationToken cancellationToken)
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
            compilation.GetILifecycleHandler(),
            compilation.GetLifecycleHandler(),
            compilation.GetResolverItemsFactory(),
            compilation.GetResolverItem(),
            compilation.GetIEnumerableOfT(),
            compilation.GetIReadOnlyListOfT(),
        }.AllOrFailed();

        cancellationToken.ThrowIfCancellationRequested();
        if (requiredTypeSymbols.TryGet(out var all, out var errors))
        {
            var index = 0;
            var iInitializableType = CreateNamedType(all[index++]);
            var iAsyncInitializableType = CreateNamedType(all[index++]);

            var iDisposableType = CreateNamedType(all[index++]);
            var iAsyncDisposableType = CreateNamedType(all[index++]);

            var func = all[index++];
            var task = all[index++];

            var ilifecycleHandlerTypeSymbol = all[index++];
            var lifecycleHandlerTypeSymbol = all[index++];
            var resolverItemsFactoryTypeSymbol = all[index++];
            var resolverItemTypeSymbol = all[index++];
            var iEnumerableOfTSymbol = all[index++];
            var iReadOnlyListOfTSymbol = all[index++];
            var ilifecycleHandlerType = TypeConverter.GetNamedType(ilifecycleHandlerTypeSymbol);
            var lifecycleHandlerType = TypeConverter.GetNamedType(lifecycleHandlerTypeSymbol);
            var lifecycleHandlerConstructor = lifecycleHandlerTypeSymbol.Constructors.FirstOrDefault(x => x.Parameters.Length == 2 && x.DeclaredAccessibility == Accessibility.Public && !x.IsStatic);
            var defaultMetadata = new TypeMetadata(null, EnumerableMetadata.NonEnumerableMetadata, false);
            var lifecycleHandlerBinding = new Binding(
                lifecycleHandlerType,
                lifecycleHandlerType,
                Scope._SingleInstancePerFactory,
                new DefiniteMethod(
                    lifecycleHandlerType,
                    lifecycleHandlerType.Name,
                    lifecycleHandlerConstructor!.Parameters.Select(x => new DefiniteParameter(TypeConverter.GetNamedType((INamedTypeSymbol)x.Type), x.Name, defaultMetadata, ParameterNecessity._Optional(null))).ToValueArray(),
                    ImmutableArray<DefiniteTypeArgument>.Empty,
                    MethodKind._Constructor),
                false,
                false,
                false);
            var objectType = new NamedType(ObjectName, string.Empty, string.Empty, false);
            var intType = new NamedType(IntName, string.Empty, string.Empty, false);
            var resolverItemType = TypeConverter.GetNamedType(resolverItemTypeSymbol);
            return R.Success(new CompilationData(
                areNullableAnnotationsSupported,
                iInitializableType,
                iAsyncInitializableType,
                iDisposableType,
                iAsyncDisposableType,
                lifecycleHandlerBinding,
                GetNamedType(typeof(IGeneratedFactory)),
                GetGenericType(typeof(Sundew.Injection.Constructed<>), string.Empty),
                VoidType,
                ValueTaskType,
                GenericTypeConverter.GetGenericType(task),
                GenericTypeConverter.GetGenericType(func),
                TypeConverter.GetNamedType(resolverItemsFactoryTypeSymbol),
                resolverItemType,
                new DefiniteArrayType(resolverItemType),
                GetNamedType(typeof(Type)),
                objectType,
                intType,
                GetNamedType(typeof(IServiceProvider)),
                GetGenericType(typeof(Span<>), string.Empty).ToDefiniteClosedGenericType(ImmutableArray.Create(new DefiniteTypeArgument(objectType, new TypeMetadata(null, EnumerableMetadata.NonEnumerableMetadata, false)))),
                GetGenericType(typeof(ResolverItem), string.Empty),
                GenericTypeConverter.GetGenericType(iEnumerableOfTSymbol),
                GenericTypeConverter.GetGenericType(iEnumerableOfTSymbol),
                compilation.AssemblyName ?? string.Empty,
                TypeHelper.GetNamespace(compilation.GlobalNamespace)));
        }

        return R.Error(errors.Select(x => Diagnostic.Create(Diagnostics.RequiredTypeNotFoundError, null, x.Error)).ToValueList());
    }

    private static NamedType GetNamedType(Type type)
    {
        return new NamedType(type.Name, type.Namespace, type.Assembly.GetName().Name, type.IsValueType);
    }

    private static OpenGenericType GetGenericType(System.Type genericType, string assemblyName)
    {
        var name = GetGenericName(genericType);
        return new OpenGenericType(
            name,
            genericType.Namespace,
            assemblyName,
            genericType.GetTypeInfo().GenericTypeParameters.Select(x => new TypeParameter(x.Name)).ToImmutableArray(),
            genericType.IsValueType);
    }

    private static UnboundGenericType GetUnboundGenericType(System.Type genericType, string assemblyName)
    {
        var name = GetGenericName(genericType);
        return new UnboundGenericType(
            name,
            genericType.GetTypeInfo().GenericTypeParameters.Length,
            genericType.Namespace,
            assemblyName);
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

    private static NamedType CreateNamedType(INamedTypeSymbol namedTypeSymbol)
    {
        return new NamedType(namedTypeSymbol.MetadataName, TypeHelper.GetNamespace(namedTypeSymbol.ContainingNamespace), namedTypeSymbol.ContainingAssembly.Identity.ToString(), namedTypeSymbol.IsValueType);
    }
}