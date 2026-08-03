// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreationSource.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage;

using Sundew.Injection.Generator.TypeSystem;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record CreationSource
{
    public static CreationSource From(DefiniteMethod registrationMethod)
    {
        if (registrationMethod.IsConstructor)
        {
            return ConstructorCall(registrationMethod.ContainingType);
        }

        return StaticMethodCall(registrationMethod.ContainingType, registrationMethod);
    }
}

internal sealed partial record ArrayCreation(DefiniteType ElementType) : CreationSource;

internal sealed partial record ConstructorCall(DefiniteType Type) : CreationSource;

internal sealed partial record StaticMethodCall(DefiniteType Type, DefiniteMethod Method) : CreationSource;