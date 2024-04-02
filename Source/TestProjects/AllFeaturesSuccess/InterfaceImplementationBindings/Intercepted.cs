// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModel1.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AllFeaturesSuccess.InterfaceImplementationBindings;

using System;
using System.Collections.Generic;
using AllFeaturesSuccess.ConstructorSelection;
using AllFeaturesSuccess.MultipleImplementations;
using AllFeaturesSuccess.NewInstance;
using AllFeaturesSuccess.OverridableNew;
using AllFeaturesSuccess.UnboundType;
using AllFeaturesSuccessDependency;

public class Intercepted : IIntercepted
{
    public Resources Resources { get; }
    private readonly ISelectFactoryMethod selectFactoryMethod;
    private readonly ISelectConstructor selectConstructor;
    private readonly NewInstanceAndDisposable newInstanceAndDisposable;
    private readonly OverrideableNewImplementation overrideableNewImplementation;
    private readonly IEnumerable<IMultipleImplementationForArray> formatters;

    public Intercepted(
        IEnumerable<IMultipleImplementationForArray> formatters,
        ISelectFactoryMethod selectFactoryMethod,
        ISelectConstructor selectConstructor,
        Resources resources,
        NewInstanceAndDisposable newInstanceAndDisposable,
        OverrideableNewImplementation overrideableNewImplementation)
    {
        this.Resources = resources;
        this.selectFactoryMethod = selectFactoryMethod;
        this.selectConstructor = selectConstructor;
        this.newInstanceAndDisposable = newInstanceAndDisposable;
        this.overrideableNewImplementation = overrideableNewImplementation;
        this.formatters = formatters;
        this.Title = "T";
        this.Description = "D";
        this.Link = "L";
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }

    public string Title { get; }

    public string Description { get; }

    internal string Link { get; }
    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
        this.Resources.PrintMe(indent + 2);
        this.selectFactoryMethod.PrintMe(indent + 2);
        this.selectConstructor.PrintMe(indent + 2);
        this.newInstanceAndDisposable.PrintMe(indent + 2);
        foreach (var formatter in this.formatters)
        {
            formatter.PrintMe(indent + 2);
        }

        this.overrideableNewImplementation.PrintMe(indent + 2);
    }
}