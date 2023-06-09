﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Class1.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AllFeaturesSuccess.InterfaceImplementationBindings
{
    using System;
    using AllFeaturesSuccess.RequiredInterface;

    public class InjectableByInterface : IInjectableByInterface, IDisposable
    {
        public ISingleModuleRequiredParameterConstructorMethod SingleModuleRequiredParameter { get; }

        public InjectableByInterface(ISingleModuleRequiredParameterConstructorMethod singleModuleRequiredParameterWithItsOwnName)
        {
            this.SingleModuleRequiredParameter = singleModuleRequiredParameterWithItsOwnName;
        }

        public void PrintMe(int indent)
        {
            Console.WriteLine(new string(' ', indent) + this.GetType().Name);
            Console.WriteLine(new string(' ', indent + 2) + this.SingleModuleRequiredParameter.GetType().Name);
        }

        public void Dispose()
        {
        }
    }
}