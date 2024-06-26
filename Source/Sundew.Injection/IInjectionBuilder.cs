﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInjectionBuilder.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection;

using System;
using System.Linq.Expressions;
using Disposal.Interfaces;
using Initialization.Interfaces;
using Sundew.Injection.Interception;

public interface IInjectionBuilder
{
    /// <summary>
    /// Sets the default value for how required parameters are injected.
    /// </summary>
    Inject RequiredParameterInjection { set; }

    /// <summary>
    /// Indicates a type is a part of the required interface and should be accepted as a parameter to the generated factory.
    /// </summary>
    /// <typeparam name="TParameter">The type parameter.</typeparam>
    /// <param name="inject">Indicates how to inject the parameter.</param>
    /// <param name="scope">Indicates the scope of the parameter.</param>
    void AddParameter<TParameter>(Inject inject = Inject.Shared, Scope? scope = null);

    /// <summary>
    /// Indicates that a type and its properties are a part of the required interface and should be accepted as a parameter to the generated factory.
    /// </summary>
    /// <typeparam name="TProperties">The properties type.</typeparam>
    /// <param name="scope">Indicates the scope of the properties.</param>
    void AddParameterProperties<TProperties>(Scope? scope = null);

    /// <summary>
    /// Configures usage of the default initialization reporter.
    /// </summary>
    /// <param name="isInjectable">Indicates whether instantiation can be overruled by passing a parameter.</param>
    void UseDefaultInitializationReporter(bool isInjectable = false);

    /// <summary>
    /// Configures usage of the specified initialization reporter.
    /// </summary>
    /// <typeparam name="TInitializationReporter">The initialization reporter type.</typeparam>
    /// <param name="isInjectable">Indicates whether instantiation can be overruled by passing a parameter.</param>
    void UseInitializationReporter<TInitializationReporter>(bool isInjectable = false)
        where TInitializationReporter : IInitializationReporter;

    /// <summary>
    /// Configures usage of the default disposal reporter.
    /// </summary>
    /// <param name="isInjectable">Indicates whether instantiation can be overruled by passing a parameter.</param>
    void UseDefaultDisposalReporter(bool isInjectable = false);

    /// <summary>
    /// Configures usage of the specified disposal reporter.
    /// </summary>
    /// <typeparam name="TDisposalReporter">The disposal reporter type.</typeparam>
    /// <param name="isInjectable">Indicates whether instantiation can be overruled by passing a parameter.</param>
    void UseDisposalReporter<TDisposalReporter>(bool isInjectable = false)
        where TDisposalReporter : IDisposalReporter;

    /// <summary>
    /// Adds an interceptor.
    /// </summary>
    /// <typeparam name="TInterceptor">The interceptor type.</typeparam>
    void AddInterceptor<TInterceptor>(
        Scope? scope = null,
        Expression<Func<TInterceptor>>? constructorSelector = null)
        where TInterceptor : IInterceptor;

    /// <summary>
    /// Configures how a type should be instantiated.
    /// </summary>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="scope">Indicates the scope of the instantiated type.</param>
    /// <param name="constructorSelector">Indicates which constructor to use.</param>
    /// <param name="isInjectable">Indicates whether instantiation overruled by passing a parameter.</param>
    /// <param name="isNewOverridable">Indicates whether instantiation can be overridden.</param>
    void Bind<TImplementation>(
        Scope? scope = null,
        Expression<Func<TImplementation>>? constructorSelector = null,
        bool isInjectable = false,
        bool isNewOverridable = false);

    /// <summary>
    /// Binds an implementation to be used for an interface.
    /// </summary>
    /// <typeparam name="TInterface">The interface type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="scope">Indicates the scope of the instantiated type.</param>
    /// <param name="constructorSelector">Indicates which constructor to use.</param>
    /// <param name="isInjectable">Indicates whether instantiation overruled by passing a parameter.</param>
    /// <param name="isNewOverridable">Indicates whether instantiation can be overridden.</param>
    void Bind<TInterface, TImplementation>(
        Scope? scope = null,
        Expression<Func<TImplementation>>? constructorSelector = null,
        bool isInjectable = false,
        bool isNewOverridable = false)
        where TImplementation : TInterface;

    /// <summary>
    /// Binds an implementation to be used for an interface.
    /// </summary>
    /// <typeparam name="TInterface1">The interface1 type.</typeparam>
    /// <typeparam name="TCombinedInterface">The combined interface type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="scope">Indicates the scope of the instantiated type.</param>
    /// <param name="constructorSelector">Indicates which constructor to use.</param>
    /// <param name="isInjectable">Indicates whether instantiation overruled by passing a parameter.</param>
    /// <param name="isNewOverridable">Indicates whether instantiation can be overridden.</param>
    void Bind<TInterface1, TCombinedInterface, TImplementation>(
        Scope? scope = null,
        Expression<Func<TImplementation>>? constructorSelector = null,
        bool isInjectable = false,
        bool isNewOverridable = false)
        where TImplementation : TInterface1, TCombinedInterface
        where TCombinedInterface : TInterface1;

    /// <summary>
    /// Binds an implementation to be used for an interface.
    /// </summary>
    /// <typeparam name="TInterface1">The interface1 type.</typeparam>
    /// <typeparam name="TInterface2">The interface2 type.</typeparam>
    /// <typeparam name="TCombinedInterface">The combined interface type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="scope">Indicates the scope of the instantiated type.</param>
    /// <param name="constructorSelector">Indicates which constructor to use.</param>
    /// <param name="isInjectable">Indicates whether instantiation overruled by passing a parameter.</param>
    /// <param name="isNewOverridable">Indicates whether instantiation can be overridden.</param>
    void Bind<TInterface1, TInterface2, TCombinedInterface, TImplementation>(
        Scope? scope = null,
        Expression<Func<TImplementation>>? constructorSelector = null,
        bool isInjectable = false,
        bool isNewOverridable = false)
        where TImplementation : TInterface1, TInterface2, TCombinedInterface
        where TCombinedInterface : TInterface1, TInterface2;

    /// <summary>
    /// Binds an implementation to be used for an interface.
    /// </summary>
    /// <typeparam name="TInterface1">The interface1 type.</typeparam>
    /// <typeparam name="TInterface2">The interface2 type.</typeparam>
    /// <typeparam name="TInterface3">The interface3 type.</typeparam>
    /// <typeparam name="TCombinedInterface">The combined interface type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="scope">Indicates the scope of the instantiated type.</param>
    /// <param name="constructorSelector">Indicates which constructor to use.</param>
    /// <param name="isInjectable">Indicates whether instantiation overruled by passing a parameter.</param>
    /// <param name="isNewOverridable">Indicates whether instantiation can be overridden in derived factory.</param>
    void Bind<TInterface1, TInterface2, TInterface3, TCombinedInterface, TImplementation>(
        Scope? scope = null,
        Expression<Func<TImplementation>>? constructorSelector = null,
        bool isInjectable = false,
        bool isNewOverridable = false)
        where TImplementation : TInterface1, TInterface2, TInterface3, TCombinedInterface
        where TCombinedInterface : TInterface1, TInterface2, TInterface3;

    /// <summary>
    /// Binds an implementation to be used for an interface.
    /// </summary>
    /// <typeparam name="TInterface1">The interface1 type.</typeparam>
    /// <typeparam name="TInterface2">The interface2 type.</typeparam>
    /// <typeparam name="TInterface3">The interface3 type.</typeparam>
    /// <typeparam name="TInterface4">The interface4 type.</typeparam>
    /// <typeparam name="TCombinedInterface">The combined interface type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="scope">Indicates the scope of the instantiated type.</param>
    /// <param name="constructorSelector">Indicates which constructor to use.</param>
    /// <param name="isInjectable">Indicates whether instantiation overruled by passing a parameter.</param>
    /// <param name="isNewOverridable">Indicates whether instantiation can be overridden in derived factory.</param>
    void Bind<TInterface1, TInterface2, TInterface3, TInterface4, TCombinedInterface, TImplementation>(
        Scope? scope = null,
        Expression<Func<TImplementation>>? constructorSelector = null,
        bool isInjectable = false,
        bool isNewOverridable = false)
        where TImplementation : TInterface1, TInterface2, TInterface3, TInterface4, TCombinedInterface
        where TCombinedInterface : TInterface1, TInterface2, TInterface3, TInterface4;

    /// <summary>
    /// Configures how a type should be instantiated.
    /// </summary>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="scope">Indicates the scope of the instantiated type.</param>
    /// <param name="constructorSelector">Indicates which constructor to use.</param>
    /// <param name="isNewOverridable">Indicates whether instantiation can be overridden.</param>
    void BindGeneric<TImplementation>(
        Scope? scope = null,
        Expression<Func<TImplementation>>? constructorSelector = null,
        bool isNewOverridable = false);

    /// <summary>
    /// Binds an implementation to be used for an interface.
    /// </summary>
    /// <typeparam name="TInterface1">The interface1 type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="scope">Indicates the scope of the instantiated type.</param>
    /// <param name="constructorSelector">Indicates which constructor to use.</param>
    /// <param name="isNewOverridable">Indicates whether instantiation can be overridden.</param>
    void BindGeneric<TInterface1, TImplementation>(
        Scope? scope = null,
        Expression<Func<TImplementation>>? constructorSelector = null,
        bool isNewOverridable = false)
        where TImplementation : TInterface1;

    /// <summary>
    /// Binds an implementation to be used for an interface.
    /// </summary>
    /// <typeparam name="TInterface1">The interface1 type.</typeparam>
    /// <typeparam name="TInterface2">The interface2 type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="scope">Indicates the scope of the instantiated type.</param>
    /// <param name="constructorSelector">Indicates which constructor to use.</param>
    /// <param name="isNewOverridable">Indicates whether instantiation can be overridden.</param>
    void BindGeneric<TInterface1, TInterface2, TImplementation>(
        Scope? scope = null,
        Expression<Func<TImplementation>>? constructorSelector = null,
        bool isNewOverridable = false)
        where TImplementation : TInterface1, TInterface2;

    /// <summary>
    /// Binds an implementation to be used for an interface.
    /// </summary>
    /// <typeparam name="TInterface1">The interface1 type.</typeparam>
    /// <typeparam name="TInterface2">The interface2 type.</typeparam>
    /// <typeparam name="TInterface3">The interface3 type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="scope">Indicates the scope of the instantiated type.</param>
    /// <param name="constructorSelector">Indicates which constructor to use.</param>
    /// <param name="isNewOverridable">Indicates whether instantiation can be overridden.</param>
    void BindGeneric<TInterface1, TInterface2, TInterface3, TImplementation>(
        Scope? scope = null,
        Expression<Func<TImplementation>>? constructorSelector = null,
        bool isNewOverridable = false)
        where TImplementation : TInterface1, TInterface2, TInterface3;

    /// <summary>
    /// Binds an implementation to be used for an interface.
    /// </summary>
    /// <typeparam name="TInterface1">The interface1 type.</typeparam>
    /// <typeparam name="TInterface2">The interface2 type.</typeparam>
    /// <typeparam name="TInterface3">The interface3 type.</typeparam>
    /// <typeparam name="TInterface4">The interface4 type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="scope">Indicates the scope of the instantiated type.</param>
    /// <param name="constructorSelector">Indicates which constructor to use.</param>
    /// <param name="isNewOverridable">Indicates whether instantiation can be overridden.</param>
    void BindGeneric<TInterface1, TInterface2, TInterface3, TInterface4, TImplementation>(
        Scope? scope = null,
        Expression<Func<TImplementation>>? constructorSelector = null,
        bool isNewOverridable = false)
        where TImplementation : TInterface1, TInterface2, TInterface3, TInterface4;

    void BindFactory<TFactory>(
        Func<IFactoryMethodBindingSelector<TFactory>, IFactoryMethodBindings<TFactory>> factoryMethods);

    void BindFactory<TFactory>(
        Expression<Func<TFactory, object>> factoryMethodSelector);

    void BindFactory<TFactory>()
        where TFactory : IGeneratedFactory;

    /// <summary>
    /// Configures interception for a given type.
    /// </summary>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    void Intercept<TImplementation>();

    /// <summary>
    /// Configures interception for a given type.
    /// </summary>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="methods">Specifies whether the list of functions are included or excluded for interception.</param>
    /// <param name="func">A func to be intercepted (or excluded).</param>
    /// <param name="funcs">Funcs to be intercepted (or excluded).</param>
    void Intercept<TImplementation>(
        Methods methods,
        Expression<Func<TImplementation, object?>> func,
        params Expression<Func<TImplementation, object?>>[] funcs);

    /// <summary>
    /// Specifies a factory to be generated and which factory methods are available.
    /// </summary>
    /// <param name="factoryMethods">A selector of the factory methods to be supported by the factory.</param>
    /// <param name="accessibility">The accessibility of the generated types.</param>
    void ImplementFactory<TFactory>(
        Func<IFactoryMethodSelector, IFactoryMethods> factoryMethods,
        Accessibility accessibility = Accessibility.Public);

    /// <summary>
    /// Specifies a factory to be generated and which factory methods are available.
    /// </summary>
    /// <param name="factoryMethods">A selector of the factory methods to be supported by the factory.</param>
    /// <param name="accessibility">The accessibility of the generated types.</param>
    void ImplementFactory<TFactory, TFactoryInterface>(
        Func<IFactoryMethodSelector, IFactoryMethods> factoryMethods,
        Accessibility accessibility = Accessibility.Public)
        where TFactory : TFactoryInterface;

    /// <summary>
    /// Specifies a resolver to be generated with access to the specified factories.
    /// </summary>
    /// <typeparam name="TResolver">The factory 1 type.</typeparam>
    /// <param name="factories">The factories.</param>
    /// <param name="accessibility">The accessibility of the generated types.</param>
    void ImplementServiceProvider<TResolver>(
        Func<IFactorySelector, IFactories> factories,
        Accessibility accessibility = Accessibility.Public);
}