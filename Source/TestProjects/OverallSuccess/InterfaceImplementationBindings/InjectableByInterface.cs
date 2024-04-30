// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Class1.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OverallSuccess.InterfaceImplementationBindings;

using System;
using OverallSuccess.RequiredInterface;
using OverallSuccessDependency;

public class InjectableByInterface : IInjectableByInterface, IDisposable
{
    public ISingleModuleRequiredParameterConstructorMethod SingleModuleRequiredParameter { get; }

    public InjectableByInterface(ISingleModuleRequiredParameterConstructorMethod singleModuleRequiredParameterWithItsOwnName)
    {
        this.SingleModuleRequiredParameter = singleModuleRequiredParameterWithItsOwnName;
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
        Console.WriteLine(new string(' ', indent + 2) + this.SingleModuleRequiredParameter.GetType().Name);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        FactoryLifetime.Disposed(this);
    }
}