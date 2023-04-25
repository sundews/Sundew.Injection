// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleImplementationA.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AllFeaturesSuccess.MultipleImplementations
{
    using System;
    using AllFeaturesSuccess.RequiredInterface;

    public class MultipleImplementationA : IMultipleImplementation
    {
        private readonly IMultipleModuleRequiredParameter secondSpecificallyNamedModuleParameter;

        public MultipleImplementationA(IMultipleModuleRequiredParameter secondSpecificallyNamedModuleParameter)
        {
            this.secondSpecificallyNamedModuleParameter = secondSpecificallyNamedModuleParameter;
        }

        public void Dispose()
        {
        }

        public void PrintMe(int indent)
        {
            Console.WriteLine(new string(' ', indent) + this.GetType().Name);
            Console.WriteLine(new string(' ', indent + 2) + this.secondSpecificallyNamedModuleParameter.GetType().Name);
        }
    }
}