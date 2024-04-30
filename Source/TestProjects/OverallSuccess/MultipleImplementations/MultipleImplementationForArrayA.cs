// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleImplementationForArrayA.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OverallSuccess.MultipleImplementations;

using System;
using OverallSuccess.RequiredInterface;
using OverallSuccessDependency;

public class MultipleImplementationForArrayA : IMultipleImplementationForArray
{
    private readonly IMultipleModuleRequiredParameter secondSpecificallyNamedModuleParameter;

    public MultipleImplementationForArrayA(IMultipleModuleRequiredParameter secondSpecificallyNamedModuleParameter)
    {
        this.secondSpecificallyNamedModuleParameter = secondSpecificallyNamedModuleParameter;
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
        Console.WriteLine(new string(' ', indent + 2) + this.secondSpecificallyNamedModuleParameter.GetType().Name);
    }
}