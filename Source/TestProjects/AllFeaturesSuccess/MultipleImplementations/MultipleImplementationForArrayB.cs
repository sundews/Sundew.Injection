// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleImplementationForArrayB.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AllFeaturesSuccess.MultipleImplementations;

using System;
using AllFeaturesSuccess.RequiredInterface;
using AllFeaturesSuccessDependency;

public class MultipleImplementationForArrayB : IMultipleImplementationForArray
{
    private readonly IMultipleModuleRequiredParameter firstSpecificallyNamedModuleParameter;

    public MultipleImplementationForArrayB(IMultipleModuleRequiredParameter firstSpecificallyNamedModuleParameter)
    {
        this.firstSpecificallyNamedModuleParameter = firstSpecificallyNamedModuleParameter;
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        FactoryLifetime.Disposed(this);
    }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
        Console.WriteLine(new string(' ', indent + 2) + this.firstSpecificallyNamedModuleParameter.GetType().Name);
    }
}