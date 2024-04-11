// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImplementationSingleInstancePerFactory.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Success.SingleInstancePerFactory;

using System;
using Success.InterfaceImplementationBindings;
using Success.InterfaceSegregationBindings;
using Success.RequiredInterface;
using SuccessDependency;

public class ImplementationSingleInstancePerFactory : IPrint
{
    private readonly IInjectedSeparately injectedSeparately;
    private readonly IInjectableByInterface injectableByInterface;
    private readonly IInterfaceSegregationOverridableNewA interfaceSegregationOverridableNewA;

    public ImplementationSingleInstancePerFactory(IInjectedSeparately injectedSeparately, IInjectableByInterface injectableByInterface, IInterfaceSegregationOverridableNewA interfaceSegregationOverridableNewA, string name)
    {
        this.injectedSeparately = injectedSeparately;
        this.injectableByInterface = injectableByInterface;
        this.interfaceSegregationOverridableNewA = interfaceSegregationOverridableNewA;
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
        Console.WriteLine(new string(' ', indent + 2) + this.injectedSeparately.GetType().Name);
        this.injectableByInterface.PrintMe(indent + 2);
        this.interfaceSegregationOverridableNewA.PrintMe(indent + 2);
    }
}